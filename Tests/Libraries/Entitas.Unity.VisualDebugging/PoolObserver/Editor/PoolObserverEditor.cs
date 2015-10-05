using Entitas;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(PoolObserverBehaviour))]
    public class PoolObserverEditor : Editor {

        public override void OnInspectorGUI() {
            var poolObserver = ((PoolObserverBehaviour)target).poolObserver;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(poolObserver.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Entities", poolObserver.pool.count.ToString());
            EditorGUILayout.LabelField("Reusable entities", poolObserver.pool.reusableEntitiesCount.ToString());
            EditorGUILayout.LabelField("Retained entities", poolObserver.pool.retainedEntitiesCount.ToString());
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create Entity")) {
                var creationIndex = poolObserver.pool.CreateEntity().creationIndex;
                var entityBehaviourName = "Entity_" + creationIndex + "()";
                var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                    .Single(eb => eb.name == entityBehaviourName);

                Selection.activeGameObject = entityBehaviour.gameObject;
            }

            var groups = poolObserver.groups;
            if (groups.Length != 0) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Groups (" + groups.Length + ")", EditorStyles.boldLabel);
                foreach (var group in groups.OrderByDescending(g => g.count)) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(group.ToString());
                    EditorGUILayout.LabelField(group.count.ToString(), GUILayout.Width(48));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            EditorUtility.SetDirty(target);
        }
    }
}