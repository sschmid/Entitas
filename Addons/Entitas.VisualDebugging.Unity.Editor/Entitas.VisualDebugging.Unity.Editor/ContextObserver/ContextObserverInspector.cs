using System.Linq;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(ContextObserverBehaviour))]
    public class ContextObserverInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var contextObserver = ((ContextObserverBehaviour)target).contextObserver;

            EditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField(contextObserver.context.contextInfo.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Entities", contextObserver.context.count.ToString());
                EditorGUILayout.LabelField("Reusable entities", contextObserver.context.reusableEntitiesCount.ToString());

                var retainedEntitiesCount = contextObserver.context.retainedEntitiesCount;
                if (retainedEntitiesCount != 0) {
                    var c = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Retained entities", retainedEntitiesCount.ToString());
                    GUI.color = c;
                    EditorGUILayout.HelpBox("WARNING: There are retained entities.\nDid you call entity.Retain(owner) and forgot to call entity.Release(owner)?", MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create Entity")) {
                        var entity = contextObserver.context.CreateEntity();
                        var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                            .Single(eb => eb.entity == entity);

                        Selection.activeGameObject = entityBehaviour.gameObject;
                    }

                    var bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Destroy All Entities")) {
                        contextObserver.context.DestroyAllEntities();
                    }
                    GUI.backgroundColor = bgColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorLayout.EndVerticalBox();

            var groups = contextObserver.groups;
            if (groups.Length != 0) {
                EditorLayout.BeginVerticalBox();
                {
                    EditorGUILayout.LabelField("Groups (" + groups.Length + ")", EditorStyles.boldLabel);
                    foreach (var group in groups.OrderByDescending(g => g.count)) {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(group.ToString());
                            EditorGUILayout.LabelField(group.count.ToString(), GUILayout.Width(48));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorLayout.EndVerticalBox();
            }

            EditorUtility.SetDirty(target);
        }
    }
}
