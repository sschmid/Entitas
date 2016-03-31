using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(DebugSystemsBehaviour))]
    public class DebugSystemsInspector : Editor {

        enum SortMethod {
            OrderOfOccurrence,

            Name,
            NameDescending,

            ExecutionTime,
            ExecutionTimeDescending
        }

        SystemsMonitor _systemsMonitor;
        Queue<float> _systemMonitorData;
        const int SYSTEM_MONITOR_DATA_LENGTH = 60;

        static bool _showInitializeSystems = true;
        static bool _showExecuteSystems = true;
        static bool _hideEmptySystems = true;
        static string _systemNameSearchTerm = string.Empty;
        
        float _threshold;
        SortMethod _systemSortMethod;

        public override void OnInspectorGUI() {
            var debugSystemsBehaviour = (DebugSystemsBehaviour)target;
            var systems = debugSystemsBehaviour.systems;

            drawSystemsOverview(systems);
            drawSystemsMonitor(systems);
            drawSystemList(systems);

            EditorUtility.SetDirty(target);
        }

        static void drawSystemsOverview(DebugSystems systems) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField(systems.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Initialize Systems", systems.initializeSystemsCount.ToString());
                EditorGUILayout.LabelField("Execute Systems", systems.executeSystemsCount.ToString());
                EditorGUILayout.LabelField("Total Systems", systems.totalSystemsCount.ToString());
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawSystemsMonitor(DebugSystems systems) {
            if (_systemsMonitor == null) {
                _systemsMonitor = new SystemsMonitor(SYSTEM_MONITOR_DATA_LENGTH);
                _systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
                if (EditorApplication.update != Repaint) {
                    EditorApplication.update += Repaint;
                }
            }

            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField("Execution duration", EditorStyles.boldLabel);

                EntitasEditorLayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Total", systems.totalDuration.ToString());

                    var buttonStyle = new GUIStyle(GUI.skin.button);
                    if (systems.paused) {
                        buttonStyle.normal = GUI.skin.button.active;
                    }
                    if (GUILayout.Button("▌▌", buttonStyle, GUILayout.Width(50))) {
                        systems.paused = !systems.paused;
                    }
                    if (GUILayout.Button("Step", GUILayout.Width(50))) {
                        systems.paused = true;
                        systems.Step();
                        addDuration((float)systems.totalDuration);
                        _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                if (!EditorApplication.isPaused && !systems.paused) {
                    addDuration((float)systems.totalDuration);
                }
                _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawSystemList(DebugSystems systems) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EntitasEditorLayout.BeginHorizontal();
                {
                    DebugSystems.avgResetInterval = (AvgResetInterval)EditorGUILayout.EnumPopup("Reset average duration Ø", DebugSystems.avgResetInterval);
                    if (GUILayout.Button("Reset Ø now", GUILayout.Width(88), GUILayout.Height(14))) {
                        systems.ResetDurations();
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                _threshold = EditorGUILayout.Slider("Threshold Ø ms", _threshold, 0f, 33f);
                _systemSortMethod = (SortMethod)EditorGUILayout.EnumPopup("Sort by ", _systemSortMethod);
                _hideEmptySystems = EditorGUILayout.Toggle("Hide empty systems", _hideEmptySystems);
                EditorGUILayout.Space();

                EntitasEditorLayout.BeginHorizontal();
                {
                    _systemNameSearchTerm = EditorGUILayout.TextField("Search", _systemNameSearchTerm);

                    const string clearButtonControlName = "Clear Button";
                    GUI.SetNextControlName(clearButtonControlName);
                    if (GUILayout.Button("x", GUILayout.Width(19), GUILayout.Height(14))) {
                        _systemNameSearchTerm = string.Empty;
                        GUI.FocusControl(clearButtonControlName);
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                _showInitializeSystems = EditorGUILayout.Foldout(_showInitializeSystems, "Initialize Systems");
                if (_showInitializeSystems && shouldShowSystems(systems, true)) {
                    EntitasEditorLayout.BeginVerticalBox();
                    {
                        var systemsDrawn = drawSystemInfos(systems, true, false);
                        if (systemsDrawn == 0) {
                            EditorGUILayout.LabelField(string.Empty);
                        }
                    }
                    EntitasEditorLayout.EndVertical();
                }

                _showExecuteSystems = EditorGUILayout.Foldout(_showExecuteSystems, "Execute Systems");
                if (_showExecuteSystems && shouldShowSystems(systems, false)) {
                    EntitasEditorLayout.BeginVerticalBox();
                    {
                        var systemsDrawn = drawSystemInfos(systems, false, false);
                        if (systemsDrawn == 0) {
                            EditorGUILayout.LabelField(string.Empty);
                        }
                    }
                    EntitasEditorLayout.EndVertical();
                }
            }
            EntitasEditorLayout.EndVertical();
        }

        int drawSystemInfos(DebugSystems systems, bool initOnly, bool isChildSysem) {
            var systemInfos = initOnly ? systems.initializeSystemInfos : systems.executeSystemInfos;
            systemInfos = systemInfos
                .Where(systemInfo => systemInfo.averageExecutionDuration >= _threshold)
                .ToArray();

            systemInfos = getSortedSystemInfos(systemInfos, _systemSortMethod);

            var systemsDrawn = 0;
            foreach (var systemInfo in systemInfos) {
                var debugSystems = systemInfo.system as DebugSystems;
                if (debugSystems != null) {
                    if (!shouldShowSystems(debugSystems, initOnly)) {
                        continue;
                    }
                }

                if (systemInfo.systemName.ToLower().Contains(_systemNameSearchTerm.ToLower())) {
                    EntitasEditorLayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(isChildSysem);
                        {
                            systemInfo.isActive = EditorGUILayout.Toggle(systemInfo.isActive, GUILayout.Width(20));
                        }
                        EditorGUI.EndDisabledGroup();
                        var reactiveSystem = systemInfo.system as ReactiveSystem;
                        if (reactiveSystem != null) {
                            if (systemInfo.isActive) {
                                reactiveSystem.Activate();
                            } else {
                                reactiveSystem.Deactivate();
                            }
                        }

                        var avg = string.Format("Ø {0:00.000}", systemInfo.averageExecutionDuration).PadRight(12);
                        var min = string.Format("▼ {0:00.000}", systemInfo.minExecutionDuration).PadRight(12);
                        var max = string.Format("▲ {0:00.000}", systemInfo.maxExecutionDuration);

                        EditorGUILayout.LabelField(systemInfo.systemName, avg + min + max, getSystemStyle(systemInfo));
                    }
                    EntitasEditorLayout.EndHorizontal();

                    systemsDrawn += 1;
                }

                var debugSystem = systemInfo.system as DebugSystems;
                if (debugSystem != null) {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    systemsDrawn += drawSystemInfos(debugSystem, initOnly, true);
                    EditorGUI.indentLevel = indent;
                }
            }

            return systemsDrawn;
        }

        static SystemInfo[] getSortedSystemInfos(SystemInfo[] systemInfos, SortMethod sortMethod) {
            if (sortMethod == SortMethod.Name) {
                return systemInfos
                    .OrderBy(systemInfo => systemInfo.systemName)
                    .ToArray();
            }
            if (sortMethod == SortMethod.NameDescending) {
                return systemInfos
                    .OrderByDescending(systemInfo => systemInfo.systemName)
                    .ToArray();
            }

            if (sortMethod == SortMethod.ExecutionTime) {
                return systemInfos
                    .OrderBy(systemInfo => systemInfo.averageExecutionDuration)
                    .ToArray();
            }
            if (sortMethod == SortMethod.ExecutionTimeDescending) {
                return systemInfos
                    .OrderByDescending(systemInfo => systemInfo.averageExecutionDuration)
                    .ToArray();
            }

            return systemInfos;
        }

        static bool shouldShowSystems(DebugSystems systems, bool initOnly) {
            if (!_hideEmptySystems) {
                return true;
            }

            if (initOnly) {
                return systems.totalInitializeSystemsCount > 0;
            }

            return systems.totalExecuteSystemsCount > 0;
        }

        static GUIStyle getSystemStyle(SystemInfo systemInfo) {
            var style = new GUIStyle(GUI.skin.label);
            var color = systemInfo.isReactiveSystems && EditorGUIUtility.isProSkin
                            ? Color.white
                            : style.normal.textColor;

            style.normal.textColor = color;

            return style;
        }

        void addDuration(float duration) {
            if (_systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH) {
                _systemMonitorData.Dequeue();
            }

            _systemMonitorData.Enqueue(duration);
        }
    }
}