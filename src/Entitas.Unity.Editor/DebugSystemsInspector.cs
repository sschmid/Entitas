using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
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
        const int SystemMonitorDataLength = 60;

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
            var systems = debugSystemsBehaviour.Systems;

            EditorGUILayout.Space();
            DrawSystemsOverview(systems);

            EditorGUILayout.Space();
            DrawSystemsMonitor(systems);

            EditorGUILayout.Space();
            DrawSystemList(systems);

            EditorGUILayout.Space();

            EditorUtility.SetDirty(target);
        }

        static void DrawSystemsOverview(DebugSystems systems)
        {
            _showDetails = EditorLayout.DrawSectionHeaderToggle("Details", _showDetails);
            if (_showDetails)
            {
                EditorLayout.BeginSectionContent();
                {
                    EditorGUILayout.LabelField(systems.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Initialize Systems", systems.TotalInitializeSystemsCount.ToString());
                    EditorGUILayout.LabelField("Execute Systems", systems.TotalExecuteSystemsCount.ToString());
                    EditorGUILayout.LabelField("Cleanup Systems", systems.TotalCleanupSystemsCount.ToString());
                    EditorGUILayout.LabelField("TearDown Systems", systems.TotalTearDownSystemsCount.ToString());
                    EditorGUILayout.LabelField("Total Systems", systems.TotalSystemsCount.ToString());
                }
                EditorLayout.EndSectionContent();
            }
        }

        void DrawSystemsMonitor(DebugSystems systems)
        {
            if (_systemsMonitor == null)
            {
                _systemsMonitor = new Graph(SystemMonitorDataLength);
                _systemMonitorData = new Queue<float>(new float[SystemMonitorDataLength]);
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
                            EditorGUILayout.LabelField("Execution duration", systems.ExecuteDuration.ToString(CultureInfo.InvariantCulture));
                            EditorGUILayout.LabelField("Cleanup duration", systems.CleanupDuration.ToString(CultureInfo.InvariantCulture));
                        }
                        EditorGUILayout.EndVertical();

                        if (_stepButtonContent == null)
                            _stepButtonContent = EditorGUIUtility.IconContent("StepButton On");

                        if (_pauseButtonContent == null)
                            _pauseButtonContent = EditorGUIUtility.IconContent("PauseButton On");

                        systems.IsPaused = GUILayout.Toggle(systems.IsPaused, _pauseButtonContent, "CommandLeft");

                        if (GUILayout.Button(_stepButtonContent, "CommandRight"))
                        {
                            systems.IsPaused = true;
                            systems.StepExecute();
                            systems.StepCleanup();
                            AddDuration((float)systems.ExecuteDuration + (float)systems.CleanupDuration);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!EditorApplication.isPaused && !systems.IsPaused)
                        AddDuration((float)systems.ExecuteDuration + (float)systems.CleanupDuration);

                    _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);
                }
                EditorLayout.EndSectionContent();
            }
        }

        void DrawSystemList(DebugSystems systems)
        {
            _showSystemsList = EditorLayout.DrawSectionHeaderToggle("Systems", _showSystemsList);
            if (_showSystemsList)
            {
                EditorLayout.BeginSectionContent();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        DebugSystems.AvgResetInterval = (AvgResetInterval)EditorGUILayout.EnumPopup("Reset average duration Ø", DebugSystems.AvgResetInterval);
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
                    if (_showInitializeSystems && ShouldShowSystems(systems, SystemInterfaceFlags.InitializeSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = DrawSystemInfos(systems, SystemInterfaceFlags.InitializeSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showExecuteSystems = EditorLayout.DrawSectionHeaderToggle("Execute Systems", _showExecuteSystems);
                    if (_showExecuteSystems && ShouldShowSystems(systems, SystemInterfaceFlags.ExecuteSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = DrawSystemInfos(systems, SystemInterfaceFlags.ExecuteSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showCleanupSystems = EditorLayout.DrawSectionHeaderToggle("Cleanup Systems", _showCleanupSystems);
                    if (_showCleanupSystems && ShouldShowSystems(systems, SystemInterfaceFlags.CleanupSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = DrawSystemInfos(systems, SystemInterfaceFlags.CleanupSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }

                    _showTearDownSystems = EditorLayout.DrawSectionHeaderToggle("TearDown Systems", _showTearDownSystems);
                    if (_showTearDownSystems && ShouldShowSystems(systems, SystemInterfaceFlags.TearDownSystem))
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            var systemsDrawn = DrawSystemInfos(systems, SystemInterfaceFlags.TearDownSystem);
                            if (systemsDrawn == 0)
                                EditorGUILayout.LabelField(string.Empty);
                        }
                        EditorLayout.EndSectionContent();
                    }
                }
                EditorLayout.EndSectionContent();
            }
        }

        int DrawSystemInfos(DebugSystems systems, SystemInterfaceFlags type)
        {
            IEnumerable<SystemInfo> systemInfos = null;

            switch (type)
            {
                case SystemInterfaceFlags.InitializeSystem:
                    systemInfos = systems.InitializeSystemInfos.Where(systemInfo => systemInfo.InitializationDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.ExecuteSystem:
                    systemInfos = systems.ExecuteSystemInfos.Where(systemInfo => systemInfo.AverageExecutionDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.CleanupSystem:
                    systemInfos = systems.CleanupSystemInfos.Where(systemInfo => systemInfo.CleanupDuration >= _threshold);
                    break;
                case SystemInterfaceFlags.TearDownSystem:
                    systemInfos = systems.TearDownSystemInfos.Where(systemInfo => systemInfo.TeardownDuration >= _threshold);
                    break;
            }

            systemInfos = GetSortedSystemInfos(systemInfos, _systemSortMethod);

            var systemsDrawn = 0;
            foreach (var systemInfo in systemInfos)
            {
                if (systemInfo.System is DebugSystems debugSystems)
                    if (!ShouldShowSystems(debugSystems, type))
                        continue;

                if (EditorLayout.MatchesSearchString(systemInfo.SystemName.ToLower(), _systemNameSearchString.ToLower()))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;

                        var wasActive = systemInfo.IsActive;
                        if (systemInfo.AreAllParentsActive)
                        {
                            systemInfo.IsActive = EditorGUILayout.Toggle(systemInfo.IsActive, GUILayout.Width(20));
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

                        if (systemInfo.IsActive != wasActive)
                        {
                            if (systemInfo.System is IReactiveSystem reactiveSystem)
                            {
                                if (systemInfo.IsActive)
                                    reactiveSystem.Activate();
                                else
                                    reactiveSystem.Deactivate();
                            }
                        }

                        switch (type)
                        {
                            case SystemInterfaceFlags.InitializeSystem:
                                EditorGUILayout.LabelField(systemInfo.SystemName, systemInfo.InitializationDuration.ToString(CultureInfo.InvariantCulture), GetSystemStyle(systemInfo, SystemInterfaceFlags.InitializeSystem));
                                break;
                            case SystemInterfaceFlags.ExecuteSystem:
                                var avgE = $"Ø {systemInfo.AverageExecutionDuration:00.000}".PadRight(12);
                                var minE = $"▼ {systemInfo.MinExecutionDuration:00.000}".PadRight(12);
                                var maxE = $"▲ {systemInfo.MaxExecutionDuration:00.000}";
                                EditorGUILayout.LabelField(systemInfo.SystemName, avgE + minE + maxE, GetSystemStyle(systemInfo, SystemInterfaceFlags.ExecuteSystem));
                                break;
                            case SystemInterfaceFlags.CleanupSystem:
                                var avgC = $"Ø {systemInfo.AverageCleanupDuration:00.000}".PadRight(12);
                                var minC = $"▼ {systemInfo.MinCleanupDuration:00.000}".PadRight(12);
                                var maxC = $"▲ {systemInfo.MaxCleanupDuration:00.000}";
                                EditorGUILayout.LabelField(systemInfo.SystemName, avgC + minC + maxC, GetSystemStyle(systemInfo, SystemInterfaceFlags.CleanupSystem));
                                break;
                            case SystemInterfaceFlags.TearDownSystem:
                                EditorGUILayout.LabelField(systemInfo.SystemName, systemInfo.TeardownDuration.ToString(CultureInfo.InvariantCulture), GetSystemStyle(systemInfo, SystemInterfaceFlags.TearDownSystem));
                                break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    systemsDrawn += 1;
                }

                if (systemInfo.System is DebugSystems debugSystem)
                {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    systemsDrawn += DrawSystemInfos(debugSystem, type);
                    EditorGUI.indentLevel = indent;
                }
            }

            return systemsDrawn;
        }

        static IEnumerable<SystemInfo> GetSortedSystemInfos(IEnumerable<SystemInfo> systemInfos, SortMethod sortMethod) => sortMethod switch
        {
            SortMethod.Name => systemInfos.OrderBy(systemInfo => systemInfo.SystemName),
            SortMethod.NameDescending => systemInfos.OrderByDescending(systemInfo => systemInfo.SystemName),
            SortMethod.ExecutionTime => systemInfos.OrderBy(systemInfo => systemInfo.AverageExecutionDuration),
            SortMethod.ExecutionTimeDescending => systemInfos.OrderByDescending(systemInfo => systemInfo.AverageExecutionDuration),
            _ => systemInfos
        };

        static bool ShouldShowSystems(DebugSystems systems, SystemInterfaceFlags type)
        {
            if (!_hideEmptySystems)
                return true;

            return type switch
            {
                SystemInterfaceFlags.InitializeSystem => systems.TotalInitializeSystemsCount > 0,
                SystemInterfaceFlags.ExecuteSystem => systems.TotalExecuteSystemsCount > 0,
                SystemInterfaceFlags.CleanupSystem => systems.TotalCleanupSystemsCount > 0,
                SystemInterfaceFlags.TearDownSystem => systems.TotalTearDownSystemsCount > 0,
                _ => true
            };
        }

        GUIStyle GetSystemStyle(SystemInfo systemInfo, SystemInterfaceFlags systemFlag)
        {
            var style = new GUIStyle(GUI.skin.label);
            var color = systemInfo.IsReactiveSystems && EditorGUIUtility.isProSkin
                ? Color.white
                : style.normal.textColor;

            if (systemFlag == SystemInterfaceFlags.ExecuteSystem && systemInfo.AverageExecutionDuration >= _systemWarningThreshold)
                color = Color.red;

            if (systemFlag == SystemInterfaceFlags.CleanupSystem && systemInfo.AverageCleanupDuration >= _systemWarningThreshold)
                color = Color.red;

            style.normal.textColor = color;

            return style;
        }

        void AddDuration(float duration)
        {
            // OnInspectorGUI is called twice per frame - only add duration once
            if (Time.renderedFrameCount != _lastRenderedFrameCount)
            {
                _lastRenderedFrameCount = Time.renderedFrameCount;
                if (_systemMonitorData.Count >= SystemMonitorDataLength)
                    _systemMonitorData.Dequeue();

                _systemMonitorData.Enqueue(duration);
            }
        }
    }
}
