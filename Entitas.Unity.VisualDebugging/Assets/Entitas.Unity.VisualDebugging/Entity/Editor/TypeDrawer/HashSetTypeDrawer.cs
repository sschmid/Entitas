using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class HashSetTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            var elementType = type.GetGenericArguments()[0];
            var itemsToRemove = new ArrayList();
            var itemsToAdd = new ArrayList();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(fieldName);
            if (GUILayout.Button("+", GUILayout.Width(19), GUILayout.Height(14))) {
                object defaultValue;
                if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    itemsToAdd.Add(defaultValue);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            foreach (var item in (IEnumerable)value) {
                EditorGUILayout.BeginHorizontal();
                var newItem = EntityDrawer.DrawAndGetNewValue(elementType, string.Empty, item, entity, index, component);
                if (EntityDrawer.DidValueChange(item, newItem)) {
                    itemsToRemove.Add(item);
                    itemsToAdd.Add(newItem);
                }

                if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                    itemsToRemove.Add(item);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel = indent;

            foreach (var item in itemsToRemove) {
                type.GetMethod("Remove").Invoke(value, new [] { item });
            }

            foreach (var item in itemsToAdd) {
                type.GetMethod("Add").Invoke(value, new [] { item });
            }

            return value;
        }
    }
}
