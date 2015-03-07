using Entitas;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(PoolDebugBehaviour))]
    public class PoolDebugEditor : Editor {

        public override void OnInspectorGUI() {
            var debugBehaviour = (PoolDebugBehaviour)target;
            var pool = debugBehaviour.pool;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Pool", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Entities", pool.Count.ToString());
            EditorGUILayout.LabelField("Reusable entities", pool.pooledEntitiesCount.ToString());
            EditorGUILayout.EndVertical();

            if (pool.groups.Count != 0) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Groups (" + pool.groups.Count + ")", EditorStyles.boldLabel);
                foreach (var group in pool.groups.Values.OrderByDescending(g => g.Count)) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(group.ToString());
                    EditorGUILayout.LabelField(group.Count.ToString(), GUILayout.Width(48));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            EditorUtility.SetDirty(target);
        }
    }
}