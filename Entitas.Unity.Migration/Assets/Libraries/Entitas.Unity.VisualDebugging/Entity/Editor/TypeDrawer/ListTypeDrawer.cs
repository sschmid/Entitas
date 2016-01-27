using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class ListTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            var list = (IList)value;
            var elementType = type.GetGenericArguments()[0];
            if (list.Count == 0) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fieldName);
                if (GUILayout.Button("Add element", GUILayout.Height(14))) {
                    object defaultValue;
                    if (EntityInspector.CreateDefault(elementType, out defaultValue)) {
                        list.Add(defaultValue);
                    }
                }
                EditorGUILayout.EndHorizontal();
            } else {
                EditorGUILayout.LabelField(fieldName);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            Action editAction = null;
            for (int i = 0; i < list.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EntityInspector.DrawAndSetElement(elementType, fieldName + "[" + i + "]", list[i],
                    entity, index, component, newValue => list[i] = newValue);

                if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                    var removeAt = i;
                    editAction = () => list.RemoveAt(removeAt);
                }
                if (GUILayout.Button("▴", GUILayout.Width(19), GUILayout.Height(14))) {
                    object defaultValue;
                    if (EntityInspector.CreateDefault(elementType, out defaultValue)) {
                        var insertAt = i;
                        editAction = () => list.Insert(insertAt, defaultValue);
                    }
                }
                if (GUILayout.Button("▾", GUILayout.Width(19), GUILayout.Height(14))) {
                    object defaultValue;
                    if (EntityInspector.CreateDefault(elementType, out defaultValue)) {
                        var insertAt = i + 1;
                        editAction = () => list.Insert(insertAt, defaultValue);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (editAction != null) {
                editAction();
            }
            EditorGUI.indentLevel = indent;

            return list;
        }
    }
}

