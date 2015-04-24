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
        IDefaultInstanceCreator[] _defaultInstanceCreators;
        ITypeDrawer[] _typeDrawers;

        void Awake() {
            setStyles();
            var types = Assembly.GetAssembly(typeof(EntityDebugEditor)).GetTypes();
            var defaultInstanceCreators = types
                .Where(type => type.GetInterfaces().Contains(typeof(IDefaultInstanceCreator)))
                .ToArray();

            _defaultInstanceCreators = new IDefaultInstanceCreator[defaultInstanceCreators.Length];
            for (int i = 0; i < defaultInstanceCreators.Length; i++) {
                _defaultInstanceCreators[i] = (IDefaultInstanceCreator)Activator.CreateInstance(defaultInstanceCreators[i]);
            }

            var typeDrawers = types
                .Where(type => type.GetInterfaces().Contains(typeof(ITypeDrawer)))
                .ToArray();

            _typeDrawers = new ITypeDrawer[typeDrawers.Length];
            for (int i = 0; i < typeDrawers.Length; i++) {
                _typeDrawers[i] = (ITypeDrawer)Activator.CreateInstance(typeDrawers[i]);
            }
        }

        void setStyles() {
            _foldoutStyle = new GUIStyle(EditorStyles.foldout);
            _foldoutStyle.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI() {
            if (targets.Length == 1) {
                drawSingleTarget();
            } else {
                drawMultiTargets();
            }
            EditorUtility.SetDirty(target);
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
            }
        }

        object drawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, Type fieldType, object value) {
            if (value == null) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fieldName, "null");
                if (GUILayout.Button("Create", GUILayout.Height(14))) {
                    object defaultValue;
                    if (createDefault(fieldType, out defaultValue)) {
                        value = defaultValue;
                    }
                }
                EditorGUILayout.EndHorizontal();
                return value;
            }

            if (!fieldType.IsValueType) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            var typeDrawer = getTypeDrawer(fieldType);
            if (typeDrawer != null) {
                value = typeDrawer.DrawAndGetNewValue(fieldType, fieldName, value, entity, index, component);
            } else if (fieldType.IsArray) {
                value = drawAndGetArray(fieldType, fieldName, (Array)value, entity, index, component);
            } else if (fieldType.GetInterfaces().Contains(typeof(IList))) {
                value = drawAndGetList(fieldType, fieldName, (IList)value, entity, index, component);
            } else {
                EditorGUILayout.LabelField(fieldName, value.ToString());
            }

            if (!fieldType.IsValueType) {
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("x", GUILayout.Width(19), GUILayout.Height(14))) {
                    value = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            return value;
        }

        ITypeDrawer getTypeDrawer(Type type) {
            foreach (var drawer in _typeDrawers) {
                if (drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }

        object drawAndGetArray(Type type, string fieldName, Array array, Entity entity, int index, IComponent component) {
            EditorGUILayout.LabelField(fieldName);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            var elementType = type.GetElementType();
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

            return array;
        }

        object drawAndGetList(Type type, string fieldName, IList list, Entity entity, int index, IComponent component) {
            var elementType = type.GetGenericArguments()[0];
            if (list.Count == 0) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fieldName);
                if (GUILayout.Button("Add element", GUILayout.Height(14))) {
                    object defaultValue;
                    if (createDefault(elementType, out defaultValue)) {
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
                drawArrayItem(entity, index, component, list[i], elementType,
                    fieldName + "[" + i + "]", newValue => list[i] = newValue);

                if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                    var removeAt = i;
                    editAction = () => list.RemoveAt(removeAt);
                }
                if (GUILayout.Button("▴", GUILayout.Width(19), GUILayout.Height(14))) {
                    object defaultValue;
                    if (createDefault(elementType, out defaultValue)) {
                        var insertAt = i;
                        editAction = () => list.Insert(insertAt, defaultValue);
                    }
                }
                if (GUILayout.Button("▾", GUILayout.Width(19), GUILayout.Height(14))) {
                    object defaultValue;
                    if (createDefault(elementType, out defaultValue)) {
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

        void drawArrayItem(Entity entity, int index, IComponent component, object value, Type elementType, string fieldName, Action<object> setValue) {
            var newValue = drawAndGetNewValue(entity, index, component, fieldName, elementType, value);
            if (didValueChange(value, newValue)) {
                entity.WillRemoveComponent(index);
                setValue(newValue);
                entity.ReplaceComponent(index, component);
            }
        }

        bool didValueChange(object value, object newValue) {
            return (value == null && newValue != null) ||
                   (value != null && newValue == null) ||
                   ((value != null && newValue != null &&
                   !newValue.Equals(value)));
        }

        bool createDefault(Type type, out object defaultValue) {
            try {
                defaultValue = Activator.CreateInstance(type);
                return true;
            } catch (Exception) {
                foreach (var creator in _defaultInstanceCreators) {
                    if (creator.HandlesType(type)) {
                        defaultValue = creator.CreateDefault(type);
                        return true;
                    }
                }
            }

            defaultValue = null;
            return false;
        }
    }
}