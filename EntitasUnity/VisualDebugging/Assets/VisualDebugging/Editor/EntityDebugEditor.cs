using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(EntityDebugBehaviour)), CanEditMultipleObjects]
    public class EntityDebugEditor : Editor {
        GUIStyle _foldoutStyle;
        ICustomTypeDrawer[] _customTypeDrawers;

        void Awake() {
            var assembly = Assembly.GetAssembly(typeof(EntityDebugEditor));
            var customDrawers = assembly.GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(ICustomTypeDrawer)))
                .ToArray();

            _customTypeDrawers = new ICustomTypeDrawer[customDrawers.Length];
            for (int i = 0; i < customDrawers.Length; i++) {
                _customTypeDrawers[i] = (ICustomTypeDrawer)Activator.CreateInstance(customDrawers[i]);
            }
        }

        public override void OnInspectorGUI() {
            setStyles();

            if (targets.Length == 1) {
                drawSingleTarget();
            } else {
                drawMultiTargets();
            }
            EditorUtility.SetDirty(target);
        }

        void setStyles() {
            _foldoutStyle = new GUIStyle(EditorStyles.foldout);
            _foldoutStyle.fontStyle = FontStyle.Bold;
        }

        void drawSingleTarget() {
            var debugBehaviour = (EntityDebugBehaviour)target;
            var pool = debugBehaviour.pool;
            var entity = debugBehaviour.entity;

            if (GUILayout.Button("Destroy Entity")) {
                pool.DestroyEntity(entity);
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);
            if (GUILayout.Button("▸", GUILayout.Width(21), GUILayout.Height(14))) {
                debugBehaviour.FoldAllComponents();
            }
            if (GUILayout.Button("▾", GUILayout.Width(21), GUILayout.Height(14))) {
                debugBehaviour.UnfoldAllComponents();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            var indices = entity.GetComponentIndices();
            var components = entity.GetComponents();
            for (int i = 0; i < components.Length; i++) {
                drawComponent(debugBehaviour, entity, indices[i], components[i]);
            }
            EditorGUILayout.EndVertical();
        }

        void drawMultiTargets() {
            if (GUILayout.Button("Destroy selected entities")) {
                foreach (var t in targets) {
                    var debugBehaviour = (EntityDebugBehaviour)t;
                    var pool = debugBehaviour.pool;
                    var entity = debugBehaviour.entity;
                    pool.DestroyEntity(entity);
                }
            }

            EditorGUILayout.Space();

            foreach (var t in targets) {
                var debugBehaviour = (EntityDebugBehaviour)t;
                var pool = debugBehaviour.pool;
                var entity = debugBehaviour.entity;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entity.ToString());
                if (GUILayout.Button("Destroy Entity")) {
                    pool.DestroyEntity(entity);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void drawComponent(EntityDebugBehaviour debugBehaviour, Entity entity, int index, IComponent component) {
            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            if (fields.Length == 0) {
                EditorGUILayout.LabelField(type.RemoveComponentSuffix(), EditorStyles.boldLabel);
            } else {
                debugBehaviour.unfoldedComponents[index] = EditorGUILayout.Foldout(debugBehaviour.unfoldedComponents[index], type.RemoveComponentSuffix(), _foldoutStyle);
            }
            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                entity.RemoveComponent(index);
            }
            EditorGUILayout.EndHorizontal();

            if (debugBehaviour.unfoldedComponents[index]) {
                foreach (var field in fields) {
                    var value = field.GetValue(component);
                    drawField(entity, index, component, field, value);
                }
            }
            EditorGUILayout.EndVertical();
        }

        void drawField(Entity entity, int index, IComponent component, FieldInfo field, object value) {
            var newValue = drawAndGetNewValue(entity, index, component, field.Name, field.FieldType, value);
            if (didValueChange(value, newValue)) {
                entity.WillRemoveComponent(index);
                field.SetValue(component, newValue);
                entity.ReplaceComponent(index, component);
                Debug.Log("Replaced " + component + "." + field.Name + " = " + newValue);
            }
        }

        object drawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, Type fieldType, object value) {
            // Custom type support
            foreach (var drawer in _customTypeDrawers) {
                if (drawer.HandlesType(fieldType)) {
                    return drawer.DrawAndGetNewValue(entity, index, component, fieldName, value);
                }
            }

            // Unity's builtin types
            if (fieldType == typeof(Bounds))                        return EditorGUILayout.BoundsField(fieldName, (Bounds)value);
            if (fieldType == typeof(Color))                         return EditorGUILayout.ColorField(fieldName, (Color)value);
            if (fieldType == typeof(AnimationCurve))                return EditorGUILayout.CurveField(fieldName, (AnimationCurve)value);
            if (fieldType.IsEnum)                                   return EditorGUILayout.EnumPopup(fieldName, (Enum)value);
            if (fieldType == typeof(float))                         return EditorGUILayout.FloatField(fieldName, (float)value);
            if (fieldType == typeof(int))                           return EditorGUILayout.IntField(fieldName, (int)value);
            if (fieldType == typeof(Rect))                          return EditorGUILayout.RectField(fieldName, (Rect)value);
            if (fieldType == typeof(string))                        return EditorGUILayout.TextField(fieldName, (string)value);
            if (fieldType == typeof(Vector2))                       return EditorGUILayout.Vector2Field(fieldName, (Vector2)value);
            if (fieldType == typeof(Vector3))                       return EditorGUILayout.Vector3Field(fieldName, (Vector3)value);
            if (fieldType == typeof(Vector4))                       return EditorGUILayout.Vector4Field(fieldName, (Vector4)value);
            if (fieldType == typeof(bool))                          return EditorGUILayout.Toggle(fieldName, (bool)value);
            if (fieldType == typeof(UnityEngine.Object))            return EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)value, fieldType, true);
            if (fieldType.IsSubclassOf(typeof(UnityEngine.Object))) return EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)value, fieldType, true);

            if (fieldType.IsArray)                                  return drawAndGetArray(entity, index, component, fieldName, (Array)value);
            if (fieldType.GetInterfaces().Contains(typeof(IList)))  return drawAndGetList(entity, index, component, fieldName, (IList)value);

            // Anything else
            EditorGUILayout.LabelField(fieldName, value == null ? "null" : value.ToString());
            return value;
        }

        object drawAndGetArray(Entity entity, int index, IComponent component, string fieldName, Array array) {
            if (array == null) {
                EditorGUILayout.LabelField(fieldName, "null");
            } else {
                EditorGUILayout.LabelField(fieldName);
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;

                var elementType = array.GetType().GetElementType();
                if (array.Rank == 1) {
                    for (int i = 0; i < array.GetLength(0); i++) {
                        drawArrayItem(entity, index, component, array.GetValue(i), elementType,
                            fieldName + "[" + i + "]", newValue => array.SetValue(newValue, i));
                    }
                } else if (array.Rank == 2) {
                    for (int i = 0; i < array.GetLength(0); i++) {
                        for (int j = 0; j < array.GetLength(1); j++) {
                            drawArrayItem(entity, index, component, array.GetValue(i, j), elementType,
                                fieldName + "[" + i + ", " + j + "]", newValue => array.SetValue(newValue, i, j));
                        }
                        EditorGUILayout.Space();
                    }
                } else if (array.Rank == 3) {
                    for (int i = 0; i < array.GetLength(0); i++) {
                        for (int j = 0; j < array.GetLength(1); j++) {
                            for (int k = 0; k < array.GetLength(2); k++) {
                                drawArrayItem(entity, index, component, array.GetValue(i, j, k), elementType,
                                    fieldName + "[" + i + ", " + j + " ," + k + "]", newValue => array.SetValue(newValue, i, j, k));
                            }
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.Space();
                    }
                }

                EditorGUI.indentLevel = indent;
            }

            return array;
        }

        object drawAndGetList(Entity entity, int index, IComponent component, string fieldName, IList list) {
            if (list == null) {
                EditorGUILayout.LabelField(fieldName, "null");
            } else {
                EditorGUILayout.LabelField(fieldName);
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;
                var elementType = list.GetType().GetGenericArguments()[0];
                for (int i = 0; i < list.Count; i++) {
                    drawArrayItem(entity, index, component, list[i], elementType,
                        fieldName + "[" + i + "]", newValue => list[i] = newValue);
                }
                EditorGUI.indentLevel = indent;
            }

            return list;
        }

        void drawArrayItem(Entity entity, int index, IComponent component, object value, Type elementType, string fieldName, Action<object> setValue) {
            var newValue = drawAndGetNewValue(entity, index, component, fieldName, elementType, value);
            if (didValueChange(value, newValue)) {
                entity.WillRemoveComponent(index);
                setValue(newValue);
                entity.ReplaceComponent(index, component);
                Debug.Log("Replaced " + component + "." + fieldName + " = " + newValue);
            }
        }

        bool didValueChange(object value, object newValue) {
            return (value == null && newValue != null) ||
                   (value != null && newValue == null) ||
                   ((value != null && newValue != null &&
                   !newValue.Equals(value)));
        }
    }
}