using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(DebugSystemsBehaviour))]
    public class DebugSystemsInspector : Editor {
        SystemsMonitor _systemsMonitor;
        Queue<float> _systemMonitorData;
        const int SYSTEM_MONITOR_DATA_LENGTH = 60;

        public override void OnInspectorGUI() {
            var debugSystemsBehaviour = (DebugSystemsBehaviour)target;
            var systems = debugSystemsBehaviour.systems;
            if (_systemsMonitor == null) {
                _systemsMonitor = new SystemsMonitor(SYSTEM_MONITOR_DATA_LENGTH);
                _systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
                if (EditorApplication.update != Repaint) {
                    EditorApplication.update += Repaint;
                }
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField(systems.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Initialize Systems", systems.initializeSystemsCount.ToString());
                EditorGUILayout.LabelField("Execute Systems", systems.executeSystemsCount.ToString());
                EditorGUILayout.LabelField("Total Systems", systems.totalSystemsCount.ToString());
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    systems.paused = EditorGUILayout.Toggle("Step manually", systems.paused);
                    EditorGUI.BeginDisabledGroup(!systems.paused);
                    if (GUILayout.Button("Step", GUILayout.Width(100))) {
                        systems.Step();
                        addDuration((float)systems.totalDuration);
                        _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Execution duration", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Total", systems.totalDuration.ToString());
                EditorGUILayout.Space();

                if (!EditorApplication.isPaused && !systems.paused) {
                    addDuration((float)systems.totalDuration);
                }
                _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);

                EditorGUILayout.BeginHorizontal();
                {
                    systems.avgResetInterval = (AvgResetInterval)EditorGUILayout.EnumPopup("Reset Ø", systems.avgResetInterval);
                    if (GUILayout.Button("Reset Ø now", GUILayout.Width(88), GUILayout.Height(14))) {
                        systems.Reset();
                    }
                }
                EditorGUILayout.EndHorizontal();

                systems.threshold = EditorGUILayout.Slider("Threshold", systems.threshold, 0f, 100f);
                EditorGUILayout.Space();

                drawSystemInfos(systems.systemInfos, false);
            }
            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(target);
        }

        void drawSystemInfos(SystemInfo[] systemInfos, bool isChildSysem) {
            var orderedSystemInfos = systemInfos
                .OrderByDescending(systemInfo => systemInfo.averageExecutionDuration)
                .ToArray();

            foreach (var systemInfo in orderedSystemInfos) {
                EditorGUILayout.BeginHorizontal();
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
                    var avg = string.Format("Ø {0:0.000}", systemInfo.averageExecutionDuration).PadRight(9);
                    var min = string.Format("min {0:0.000}", systemInfo.minExecutionDuration).PadRight(11);
                    var max = string.Format("max {0:0.000}", systemInfo.maxExecutionDuration);
                    EditorGUILayout.LabelField(systemInfo.systemName, avg + "\t" + min + "\t" + max);
                }
                EditorGUILayout.EndHorizontal();

                var debugSystem = systemInfo.system as DebugSystems;
                if (debugSystem != null) {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    drawSystemInfos(debugSystem.systemInfos, true);
                    EditorGUI.indentLevel = indent;
                }
            }
        }

        void addDuration(float duration) {
            if (_systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH) {
                _systemMonitorData.Dequeue();
            }

            _systemMonitorData.Enqueue(duration);
        }
    }
}