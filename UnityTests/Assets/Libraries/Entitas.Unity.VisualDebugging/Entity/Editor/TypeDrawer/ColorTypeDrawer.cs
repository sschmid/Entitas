using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class ColorTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Color);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.ColorField(fieldName, (Color)value);
        }
    }
}