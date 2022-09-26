using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor
{
    [CustomEditor(typeof(DebugSystemsBehaviour))]
    public class DebugSystemsInspector : UnityEditor.Editor
    {
        enum SortMethod
        {
            OrderOfOccurrence,

            Name,
            NameDescending,

            ExecutionTime,
            ExecutionTimeDescending
        }

        Graph _systemsMonitor;
        Queue<float> _systemMonitorData;
        const int SYSTEM_MONITOR_DATA_LENGTH = 60;

        static bool _showDetails = false;
        static bool _showSystemsMonitor = true;
        static bool _showSystemsList = true;

        static bool _showInitializeSystems = true;
        static bool _showExecuteSystems = true;
        static bool _showCleanupSystems = true;
        static bool _showTearDownSystems = true;
        static bool _hideEmptySystems = true;
        static string _systemNameSearchString = string.Empty;

        int _systemWarningThreshold;

        float _threshold;
        SortMethod _systemSortMethod;

        int _lastRenderedFrameCount;

        GUIContent _stepButtonContent;
        GUIContent _pauseButtonContent;

        void OnEnable()
        {
            try
            {
                var preferences = new Preferences("Entitas.properties", $"{Environment.UserName}.userproperties");
                var config = preferences.CreateAndConfigure<VisualDebuggingConfig>();
                _systemWarningThreshold = config.systemWarningThreshold;
            }
            catch (Exception)
            {
                _systemWarningThreshold = int.MaxValue;
            }
        }

        public override void OnInspectorGUI()
        {
            var debugSystemsBehaviour = (DebugSystemsBehaviour)target;
            var systems = debugSystemsBehaviour.systems;

            EditorGUILayout.Space();
            drawSystemsOverview(systems);

            EditorGUILayout.Space();
            drawSystemsMonitor(systems);

            EditorGUILayout.Space();
            drawSystemList(systems);

            EditorGUILayout.Space();

            EditorUtility.SetDirty(target);
        }

        static void drawSystemsOverview(DebugSystems systems)
        {
            _showDetails = EditorLayout.DrawSectionHeaderToggle("Details", _showDetails);
            if (_showDetails)
            {
                EditorLayout.BeginSectionContent();
                {
                    EditorGUILayout.LabelField(systems.name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Initialize Systems", systems.totalInitializeSystemsCount.ToString());
                    EditorGUILayout.LabelField("Execute Systems", systems.totalExecuteSystemsCount.ToString());
                    EditorGUILayout.LabelField("Cleanup Systems", systems.totalCleanupSystemsCount.ToString());
                    EditorGUILayout.LabelField("TearDown Systems", systems.totalTearDownSystemsCount.ToString());
                    EditorGUILayout.LabelField("Total Systems", systems.totalSystemsCount.ToString());
                }
                EditorLayout.EndSectionContent();
            }
        }

        void drawSystemsMonitor(DebugSystems systems)
        {
            if (_systemsMonitor == null)
            {
                _systemsMonitor = new Graph(SYSTEM_MONITOR_DATA_LENGTH);
                _systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
            }

            _showSystemsMonitor = EditorLayout.DrawSectionHeaderToggle("Performance", _showSystemsMonitor);
            if (_showSystemsMonitor)
            {
                EditorLayout.BeginSectionContent();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.LabelField("Execution duration", systems.executeDuration.ToString());
                            EditorGUILayout.LabelField("Cleanup duration", systems.cleanupDuration.ToString());
                        }
                        EditorGUILayout.EndVertical();

                        if (_stepButtonContent == null)
                            _stepButtonContent = EditorGUIUtility.IconContent("StepButton On");

                        if (_pauseButtonContent == null)
                            _pauseButtonContent = EditorGUIUtility.IconContent("PauseButton On");

                        systems.paused = GUILayout.Toggle(systems.paused, _pauseButtonContent, "CommandLeft");

                        if (GUILayout.Button(_stepButtonContent, "CommandRight"))
                        {
                            systems.paused = true;
                            systems.StepExecute();
                            systems.StepCleanup();
                            addDuration((float)systems.executeDuration + (float)systems.cleanupDuration);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!EditorApplication.isPaused && !systems.paused)
                        addDuration((float)systems.executeDuration + (float)systems.cleanupDuration);

                    _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);
                }
                EditorLayout.EndSectionContent();
            }
        }

        void drawSystemList(DebugSystems systems)
        {
            _showSystemsList = EditorLayout.DrawSectionHeaderToggle("Systems", _showSystemsList);
            if (_showSystemsList)
            {
                EditorLayout.BeginSectionContent();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        DebugSystems.avgResetInterval = (AvgResetInterval)EditorGUILayout.EnumPopup("Reset average duration Ø", DebugSystems.avgResetInterval);
                        if (GUILayout.Button("Reset Ø now", EditorStyles.miniButton, GUILayout.Width(88)))
                            systems.ResetDurations();
                    }
                    EditorGUILayout.EndHorizontal();

                    _threshold = EditorGUILayout.Slider("Threshold Ø ms", _threshold, 0f, 33f);

                    _hideEmptySystems = EditorGUILayout.Toggle("Hide empty systems", _hideEmptySystems);
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        _systemSortMethod = (SortMethod)EditorGUILayout.EnumPopup(_systemSortMethod, EditorStyles.popup, GUILayout.Width(150));
                        _systemNameSearchString = EditorLayout.SearchTextField(_systemNameSearchString);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    _showInitializeSystems = EditorLayout.DrawSectionHeaderToggle("Initialize Systems", _showInitializeSystems);
                    if (_showInitializeSystems && shouldShowSystems(systems, SystemInterfaceFlags.IInitializeSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = drawSystemInfos(systems, SystemInterfaceFlags.IInitializeSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showExecuteSystems = EditorLayout.DrawSectionHeaderToggle("Execute Systems", _showExecuteSystems);
                    if (_showExecuteSystems && shouldShowSystems(systems, SystemInterfaceFlags.IExecuteSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = drawSystemInfos(systems, SystemInterfaceFlags.IExecuteSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showCleanupSystems = EditorLayout.DrawSectionHeaderToggle("Cleanup Systems", _showCleanupSystems);
                    if (_showCleanupSystems && shouldShowSystems(systems, SystemInterfaceFlags.ICleanupSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = drawSystemInfos(systems, SystemInterfaceFlags.ICleanupSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showTearDownSystems = EditorLayout.DrawSectionHeaderToggle("TearDown Systems", _showTearDownSystems);
                    if (_showTearDownSystems && shouldShowSystems(systems, SystemInterfaceFlags.ITearDownSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = drawSystemInfos(systems, SystemInterfaceFlags.ITearDownSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }
                }
                EditorLayout.EndSectionContent();
            }
        }

        int drawSystemInfos(DebugSystems systems, SystemInterfaceFlags type)
        {
            IEnumerable<SystemInfo> systemInfos = null;

            switch (type)
            {
                case SystemInterfaceFlags.IInitializeSystem:
                    systemInfos = systems.initializeSystemInfos.Where(systemInfo => systemInfo.initializationDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.IExecuteSystem:
                    systemInfos = systems.executeSystemInfos.Where(systemInfo => systemInfo.averageExecutionDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.ICleanupSystem:
                    systemInfos = systems.cleanupSystemInfos.Where(systemInfo => systemInfo.cleanupDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.ITearDownSystem:
                    systemInfos = systems.tearDownSystemInfos.Where(systemInfo => systemInfo.teardownDuration >= _threshold);
                    break;
            }

            systemInfos = getSortedSystemInfos(systemInfos, _systemSortMethod);

            var systemsDrawn = 0;
            foreach (var systemInfo in systemInfos)
            {
                if (systemInfo.system is DebugSystems debugSystems)
                    if (!shouldShowSystems(debugSystems, type))
                        continue;

                if (EditorLayout.MatchesSearchString(systemInfo.systemName.ToLower(), _systemNameSearchString.ToLower()))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;

                        var wasActive = systemInfo.isActive;
                        if (systemInfo.areAllParentsActive)
                        {
                            systemInfo.isActive = EditorGUILayout.Toggle(systemInfo.isActive, GUILayout.Width(20));
                        }
                        else
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            {
                                EditorGUILayout.Toggle(false, GUILayout.Width(20));
                            }
                        }

                        EditorGUI.EndDisabledGroup();

                        EditorGUI.indentLevel = indent;

                        if (systemInfo.isActive != wasActive)
                        {
                            var reactiveSystem = systemInfo.system as IReactiveSystem;
                            if (reactiveSystem != null)
                            {
                                if (systemInfo.isActive)
                                    reactiveSystem.Activate();
                                else
                                    reactiveSystem.Deactivate();
                            }
                        }

                        switch (type)
                        {
                            case SystemInterfaceFlags.IInitializeSystem:
                                EditorGUILayout.LabelField(systemInfo.systemName, systemInfo.initializationDuration.ToString(), getSystemStyle(systemInfo, SystemInterfaceFlags.IInitializeSystem));
                                break;
                            case SystemInterfaceFlags.IExecuteSystem:
                                var avgE = $"Ø {systemInfo.averageExecutionDuration:00.000}".PadRight(12);
                                var minE = $"▼ {systemInfo.minExecutionDuration:00.000}".PadRight(12);
                                var maxE = $"▲ {systemInfo.maxExecutionDuration:00.000}";
                                EditorGUILayout.LabelField(systemInfo.systemName, avgE + minE + maxE, getSystemStyle(systemInfo, SystemInterfaceFlags.IExecuteSystem));
                                break;
                            case SystemInterfaceFlags.ICleanupSystem:
                                var avgC = $"Ø {systemInfo.averageCleanupDuration:00.000}".PadRight(12);
                                var minC = $"▼ {systemInfo.minCleanupDuration:00.000}".PadRight(12);
                                var maxC = $"▲ {systemInfo.maxCleanupDuration:00.000}";
                                EditorGUILayout.LabelField(systemInfo.systemName, avgC + minC + maxC, getSystemStyle(systemInfo, SystemInterfaceFlags.ICleanupSystem));
                                break;
                            case SystemInterfaceFlags.ITearDownSystem:
                                EditorGUILayout.LabelField(systemInfo.systemName, systemInfo.teardownDuration.ToString(), getSystemStyle(systemInfo, SystemInterfaceFlags.ITearDownSystem));
                                break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    systemsDrawn += 1;
                }

                if (systemInfo.system is DebugSystems debugSystem)
                {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    systemsDrawn += drawSystemInfos(debugSystem, type);
                    EditorGUI.indentLevel = indent;
                }
            }

            return systemsDrawn;
        }

        static IEnumerable<SystemInfo> getSortedSystemInfos(IEnumerable<SystemInfo> systemInfos, SortMethod sortMethod) => sortMethod switch
        {
            SortMethod.Name => systemInfos.OrderBy(systemInfo => systemInfo.systemName),
            SortMethod.NameDescending => systemInfos.OrderByDescending(systemInfo => systemInfo.systemName),
            SortMethod.ExecutionTime => systemInfos.OrderBy(systemInfo => systemInfo.averageExecutionDuration),
            SortMethod.ExecutionTimeDescending => systemInfos.OrderByDescending(systemInfo => systemInfo.averageExecutionDuration),
            _ => systemInfos
        };

        static bool shouldShowSystems(DebugSystems systems, SystemInterfaceFlags type)
        {
            if (!_hideEmptySystems)
                return true;

            return type switch
            {
                SystemInterfaceFlags.IInitializeSystem => systems.totalInitializeSystemsCount > 0,
                SystemInterfaceFlags.IExecuteSystem => systems.totalExecuteSystemsCount > 0,
                SystemInterfaceFlags.ICleanupSystem => systems.totalCleanupSystemsCount > 0,
                SystemInterfaceFlags.ITearDownSystem => systems.totalTearDownSystemsCount > 0,
                _ => true
            };
        }

        GUIStyle getSystemStyle(SystemInfo systemInfo, SystemInterfaceFlags systemFlag)
        {
            var style = new GUIStyle(GUI.skin.label);
            var color = systemInfo.isReactiveSystems && EditorGUIUtility.isProSkin
                ? Color.white
                : style.normal.textColor;

            if (systemFlag == SystemInterfaceFlags.IExecuteSystem && systemInfo.averageExecutionDuration >= _systemWarningThreshold)
                color = Color.red;

            if (systemFlag == SystemInterfaceFlags.ICleanupSystem && systemInfo.averageCleanupDuration >= _systemWarningThreshold)
                color = Color.red;

            style.normal.textColor = color;

            return style;
        }

        void addDuration(float duration)
        {
            // OnInspectorGUI is called twice per frame - only add duration once
            if (Time.renderedFrameCount != _lastRenderedFrameCount)
            {
                _lastRenderedFrameCount = Time.renderedFrameCount;
                if (_systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH)
                    _systemMonitorData.Dequeue();

                _systemMonitorData.Enqueue(duration);
            }
        }
    }
}
