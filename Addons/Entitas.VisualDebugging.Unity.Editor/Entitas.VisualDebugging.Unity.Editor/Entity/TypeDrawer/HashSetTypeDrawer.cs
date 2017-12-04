using System;
using System.Collections;
using System.Collections.Generic;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class HashSetTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            var elementType = memberType.GetGenericArguments()[0];
            var itemsToRemove = new ArrayList();
            var itemsToAdd = new ArrayList();
            var isEmpty = !((IEnumerable)value).GetEnumerator().MoveNext();

            EditorGUILayout.BeginHorizontal();
            {
                if (isEmpty) {
                    EditorGUILayout.LabelField(memberName, "empty");
                } else {
                    EditorGUILayout.LabelField(memberName);
                }

                if (EditorLayout.MiniButton("new " + elementType.ToCompilableString().ShortTypeName())) {
                    object defaultValue;
                    if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                        itemsToAdd.Add(defaultValue);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!isEmpty) {
                EditorGUILayout.Space();
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;
                foreach (var item in (IEnumerable)value) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EntityDrawer.DrawObjectMember(elementType, string.Empty, item,
                            target, (newComponent, newValue) => {
                                itemsToRemove.Add(item);
                                itemsToAdd.Add(newValue);
                            });

                        if (EditorLayout.MiniButton("-")) {
                            itemsToRemove.Add(item);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel = indent;
            }

            foreach (var item in itemsToRemove) {
                memberType.GetMethod("Remove").Invoke(value, new[] { item });
            }

            foreach (var item in itemsToAdd) {
                memberType.GetMethod("Add").Invoke(value, new[] { item });
            }

            return value;
        }
    }
}
