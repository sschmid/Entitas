using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(SystemsDebugBehaviour))]
    public class SystemsDebugEditor : Editor {
        SystemMonitorEditor _systemMonitor;
        List<float> _systemMonitorData;

        public override void OnInspectorGUI() {
            var debugBehaviour = (SystemsDebugBehaviour)target;
            var systems = debugBehaviour.systems;
            if (_systemMonitor == null) {
                _systemMonitor = new SystemMonitorEditor(this, 80f);
                _systemMonitorData = new List<float>();
                if (EditorApplication.update != Repaint) {
                    EditorApplication.update += Repaint;
                }
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(systems.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Start Systems", systems.startSystemsCount.ToString());
            EditorGUILayout.LabelField("Execute Systems", systems.executeSystemsCount.ToString());
            EditorGUILayout.LabelField("Total Systems", systems.totalSystemsCount.ToString());
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            systems.paused = EditorGUILayout.Toggle("Step manually", systems.paused);
            GUI.enabled = systems.paused;
            if (GUILayout.Button("Step", GUILayout.Width(100))) {
                systems.Step();
                addDuration((float)systems.totalDuration);
                _systemMonitor.Draw(_systemMonitorData);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Execution duration", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total", systems.totalDuration.ToString());
            EditorGUILayout.Space();

            if (!EditorApplication.isPaused && !systems.paused) {
                addDuration((float)systems.totalDuration);
            }
            _systemMonitor.Draw(_systemMonitorData);

            EditorGUILayout.BeginHorizontal();
            systems.avgResetInterval = (AvgResetInterval)EditorGUILayout.EnumPopup("Reset Ø", systems.avgResetInterval);
            if (GUILayout.Button("Reset Ø now", GUILayout.Width(88), GUILayout.Height(14))) {
                systems.Reset();
            }
            EditorGUILayout.EndHorizontal();

            systems.threshold = EditorGUILayout.Slider("Threshold", systems.threshold, 0f, 100f);
            EditorGUILayout.Space();
            var orderedSystemInfos = systems.systemInfos
                .OrderByDescending(systemInfo => systemInfo.averageExecutionDuration)
                .ToArray();
            foreach (var systemInfo in orderedSystemInfos) {
                EditorGUILayout.BeginHorizontal();
                systemInfo.isActive = EditorGUILayout.Toggle(systemInfo.isActive, GUILayout.Width(20));
                var avg = string.Format("Ø {0:0.000}", systemInfo.averageExecutionDuration).PadRight(9);
                var min = string.Format("min {0:0.000}", systemInfo.minExecutionDuration).PadRight(11);
                var max = string.Format("max {0:0.000}", systemInfo.maxExecutionDuration);
                EditorGUILayout.LabelField(systemInfo.systemName, avg + "\t" + min + "\t" + max);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(target);
        }

        void addDuration(float duration) {
            _systemMonitorData.Add(duration);
            if (_systemMonitorData.Count > 60) {
                _systemMonitorData.RemoveAt(0);
            }
        }
    }
}