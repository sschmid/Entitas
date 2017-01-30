using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Entitas.Unity.VisualDebugging {

    [CustomEditor(typeof(ContextObserverBehaviour))]
    public class ContextObserverInspector : Editor {

        public override void OnInspectorGUI() {
            var contextObserver = ((ContextObserverBehaviour)target).contextObserver;

            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField(contextObserver.context.contextInfo.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Entities", contextObserver.context.count.ToString());
                EditorGUILayout.LabelField("Reusable entities", contextObserver.context.reusableEntitiesCount.ToString());

                var retainedEntitiesCount = contextObserver.context.retainedEntitiesCount;
                if(retainedEntitiesCount != 0) {
                    var c = GUI.contentColor;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Retained entities", retainedEntitiesCount.ToString());
                    GUI.color = c;
                    EditorGUILayout.HelpBox("WARNING: There are retained entities.\nDid you call entity.Retain(owner) and forgot to call entity.Release(owner)?", MessageType.Warning);
                }

                EntitasEditorLayout.BeginHorizontal();
                {
                    if(GUILayout.Button("Create Entity")) {
                        // TODO
                        //var entity = contextObserver.context.CreateEntity();
                        //var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                        //                            .Single(eb => eb.entity == entity);

                        //Selection.activeGameObject = entityBehaviour.gameObject;
                    }

                    var bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if(GUILayout.Button("Destroy All Entities")) {
                        contextObserver.context.DestroyAllEntities();
                    }
                    GUI.backgroundColor = bgColor;
                }
                EntitasEditorLayout.EndHorizontal();
            }
            EntitasEditorLayout.EndVertical();

            var groups = contextObserver.groups;
            if(groups.Length != 0) {
                EntitasEditorLayout.BeginVerticalBox();
                {
                    EditorGUILayout.LabelField("Groups (" + groups.Length + ")", EditorStyles.boldLabel);
                    foreach(var group in groups.OrderByDescending(g => g.count)) {
                        EntitasEditorLayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(group.ToString());
                            EditorGUILayout.LabelField(group.count.ToString(), GUILayout.Width(48));
                        }
                        EntitasEditorLayout.EndHorizontal();
                    }
                    if(GUILayout.Button("Clear Groups")) {
                        contextObserver.context.ClearGroups();
                    }
                }
                EntitasEditorLayout.EndVertical();
            }

            EditorUtility.SetDirty(target);
        }
    }
}
