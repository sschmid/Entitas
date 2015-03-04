using System;
using System.Reflection;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Debug {
    [CustomEditor(typeof(EntityDebugBehaviour)), CanEditMultipleObjects]
    public class EntityDebugEditor : Editor {
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
            EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);

            var indices = entity.GetComponentIndices();
            var components = entity.GetComponents();
            for (int i = 0; i < components.Length; i++) {
                EditorGUILayout.BeginVertical(GUI.skin.button);
                var index = indices[i];
                var component = components[i];
                drawComponent(entity, index, component);
                EditorGUILayout.EndVertical();
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

        void drawComponent(Entity entity, int index, IComponent component) {
            var type = component.GetType();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                entity.RemoveComponent(index);
            }
            EditorGUILayout.EndHorizontal();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields) {
                var value = field.GetValue(component);
                drawField(entity, index, component, field, value);
            }
        }

        void drawField(Entity entity, int index, IComponent component, FieldInfo field, object value) {
            var currentValue = value;
            var newValue = value;

            // Unity's builtin types
            if (field.FieldType == typeof(Bounds))              newValue = EditorGUILayout.BoundsField(field.Name, (Bounds)currentValue);
            else if (field.FieldType == typeof(Color))          newValue = EditorGUILayout.ColorField(field.Name, (Color)currentValue);
            else if (field.FieldType == typeof(AnimationCurve)) newValue = EditorGUILayout.CurveField(field.Name, (AnimationCurve)currentValue);
            else if (field.FieldType.IsEnum)                    newValue = EditorGUILayout.EnumPopup(field.Name, (Enum)currentValue);
            else if (field.FieldType == typeof(float))          newValue = EditorGUILayout.FloatField(field.Name, (float)currentValue);
            else if (field.FieldType == typeof(int))            newValue = EditorGUILayout.IntField(field.Name, (int)currentValue);
            else if (field.FieldType == typeof(Rect))           newValue = EditorGUILayout.RectField(field.Name, (Rect)currentValue);
            else if (field.FieldType == typeof(string))         newValue = EditorGUILayout.TextField(field.Name, (string)currentValue);
            else if (field.FieldType == typeof(Vector2))        newValue = EditorGUILayout.Vector2Field(field.Name, (Vector2)currentValue);
            else if (field.FieldType == typeof(Vector3))        newValue = EditorGUILayout.Vector3Field(field.Name, (Vector3)currentValue);
            else if (field.FieldType == typeof(Vector4))        newValue = EditorGUILayout.Vector4Field(field.Name, (Vector4)currentValue);
            else if (field.FieldType == typeof(bool))           newValue = EditorGUILayout.Toggle(field.Name, (bool)currentValue);
            else if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                newValue = EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)currentValue, field.FieldType, true);

            // Custom type support
            else if (field.FieldType == typeof(DateTime))       newValue = DateTime.Parse(EditorGUILayout.TextField(field.Name, ((DateTime)currentValue).ToString()));

            // Anything else
            else EditorGUILayout.LabelField(field.Name, currentValue == null ? "null" : currentValue.ToString());

            var changed = (currentValue == null && newValue != null) ||
                          (currentValue != null && newValue == null) ||
                          ((currentValue != null && newValue != null &&
                          !newValue.Equals(currentValue)));

            if (changed) {
                entity.WillRemoveComponent(index);
                field.SetValue(component, newValue);
                entity.ReplaceComponent(index, component);
            }
        }
    }
}