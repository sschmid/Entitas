using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Entitas.Unity.VisualDebugging {
    public class ArrayTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type.IsArray;
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            var array = (Array)value;
            var elementType = type.GetElementType();
            var indent = EditorGUI.indentLevel;
            if (array.Rank == 1) {
                if (array.GetLength(0) == 0) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(fieldName);
                    if (GUILayout.Button("Add element", GUILayout.Height(14))) {
                        object defaultValue;
                        if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                            var newArray = Array.CreateInstance(elementType, 1);
                            newArray.SetValue(defaultValue, 0);
                            array = newArray;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                } else {
                    EditorGUILayout.LabelField(fieldName);
                }
                EditorGUI.indentLevel = indent + 1;

                Func<Array> editAction = null;
                for (int i = 0; i < array.GetLength(0); i++) {
                    EditorGUILayout.BeginHorizontal();
                    EntityDrawer.DrawAndSetElement(elementType, fieldName + "[" + i + "]", array.GetValue(i),
                        entity, index, component, newValue => array.SetValue(newValue, i));

                    var localIndex = i;
                    var action = drawEditActions(array, elementType, localIndex);
                    if (action != null) {
                        editAction = action;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (editAction != null) {
                    array = editAction();
                }
            } else if (array.Rank == 2) {
                EditorGUILayout.LabelField(fieldName);
                for (int i = 0; i < array.GetLength(0); i++) {
                    for (int j = 0; j < array.GetLength(1); j++) {
                        EntityDrawer.DrawAndSetElement(elementType, fieldName + "[" + i + ", " + j + "]", array.GetValue(i, j),
                            entity, index, component, newValue => array.SetValue(newValue, i, j));
                    }
                    EditorGUILayout.Space();
                }
            } else if (array.Rank == 3) {
                EditorGUILayout.LabelField(fieldName);
                for (int i = 0; i < array.GetLength(0); i++) {
                    for (int j = 0; j < array.GetLength(1); j++) {
                        for (int k = 0; k < array.GetLength(2); k++) {
                            EntityDrawer.DrawAndSetElement(elementType, fieldName + "[" + i + ", " + j + " ," + k + "]", array.GetValue(i, j, k),
                                entity, index, component, newValue => array.SetValue(newValue, i, j, k));
                        }
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.Space();
                }
            }

            EditorGUI.indentLevel = indent;

            return array;
        }

        static Func<Array> drawEditActions(Array array, Type elementType, int index) {
            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                return () => arrayRemoveAt(array, elementType, index);
            }
            if (GUILayout.Button("▴", GUILayout.Width(19), GUILayout.Height(14))) {
                object defaultValue;
                if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    return () => arrayInsertAt(array, elementType, defaultValue, index);
                }
            }
            if (GUILayout.Button("▾", GUILayout.Width(19), GUILayout.Height(14))) {
                object defaultValue;
                if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    return () => arrayInsertAt(array, elementType, defaultValue, index + 1);
                }
            }

            return null;
        }

        static Array arrayRemoveAt(Array array, Type elementType, int removeAt) {
            var arrayList = new ArrayList(array);
            arrayList.RemoveAt(removeAt);
            return arrayList.ToArray(elementType);
        }

        static Array arrayInsertAt(Array array, Type elementType, object value, int insertAt) {
            var arrayList = new ArrayList(array);
            arrayList.Insert(insertAt, value);
            return arrayList.ToArray(elementType);
        }
    }
}

