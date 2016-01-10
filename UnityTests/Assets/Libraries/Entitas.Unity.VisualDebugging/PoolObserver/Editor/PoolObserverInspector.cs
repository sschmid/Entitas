using Entitas;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(PoolObserverBehaviour))]
    public class PoolObserverInspector : Editor {

        public override void OnInspectorGUI() {
            var poolObserver = ((PoolObserverBehaviour)target).poolObserver;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(poolObserver.pool.metaData.poolName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Entities", poolObserver.pool.count.ToString());
            EditorGUILayout.LabelField("Reusable entities", poolObserver.pool.reusableEntitiesCount.ToString());

            var retainedEntitiesCount = poolObserver.pool.retainedEntitiesCount;
            if (retainedEntitiesCount != 0) {
                var c = GUI.contentColor;
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Retained entities", retainedEntitiesCount.ToString());
                GUI.color = c;
                EditorGUILayout.HelpBox("WARNING: There are retained entities.\nDid you call entity.Retain(owner) and forgot to call entity.Release(owner)?", MessageType.Warning);
            } else {
                EditorGUILayout.LabelField("Retained entities", retainedEntitiesCount.ToString());
            }

            if (GUILayout.Button("Create Entity")) {
                var entity = poolObserver.pool.CreateEntity();
                var entityBehaviourName = "Entity_" + entity.creationIndex + "(" + entity.retainCount + ")()";
                var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                    .Single(eb => eb.name == entityBehaviourName);

                Selection.activeGameObject = entityBehaviour.gameObject;
            }

            if (GUILayout.Button("Destroy All Entities")) {
                poolObserver.pool.DestroyAllEntities();
            }

            EditorGUILayout.EndVertical();

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