using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(SystemsDebugBehaviour))]
    public class SystemsDebugEditor : Editor {

        public override void OnInspectorGUI() {
            var debugBehaviour = (SystemsDebugBehaviour)target;
            var systems = debugBehaviour.systems;

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
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Execution duration", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total", systems.totalDuration.ToString());
            EditorGUILayout.Space();

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
    }
}