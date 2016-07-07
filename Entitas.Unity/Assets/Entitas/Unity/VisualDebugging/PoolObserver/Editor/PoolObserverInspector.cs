using Entitas;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(PoolObserverBehaviour))]
    public class PoolObserverInspector : Editor {

        public override void OnInspectorGUI() {
            var poolObserver = ((PoolObserverBehaviour)target).poolObserver;

            EntitasEditorLayout.BeginVerticalBox();
            {
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
                }

                EntitasEditorLayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create Entity")) {
                        var entity = poolObserver.pool.CreateEntity();
                        var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                                                    .Single(eb => eb.entity == entity);

                        Selection.activeGameObject = entityBehaviour.gameObject;
                    }

                    var bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Destroy All Entities")) {
                        poolObserver.pool.DestroyAllEntities();
                    }
                    GUI.backgroundColor = bgColor;
                }
                EntitasEditorLayout.EndHorizontal();
            }
            EntitasEditorLayout.EndVertical();

            var groups = poolObserver.groups;
            if (groups.Length != 0) {
                EntitasEditorLayout.BeginVerticalBox();
                {
                    EditorGUILayout.LabelField("Groups (" + groups.Length + ")", EditorStyles.boldLabel);
                    foreach (var group in groups.OrderByDescending(g => g.count)) {
                        EntitasEditorLayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(group.ToString());
                            EditorGUILayout.LabelField(group.count.ToString(), GUILayout.Width(48));
                        }
                        EntitasEditorLayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Clear Groups")) {
                        poolObserver.pool.ClearGroups();
                    }
                }
                EntitasEditorLayout.EndVertical();
            }

            EditorUtility.SetDirty(target);
        }
    }
}