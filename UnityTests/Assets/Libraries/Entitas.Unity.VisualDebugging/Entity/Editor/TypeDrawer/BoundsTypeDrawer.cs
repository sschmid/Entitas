using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class BoundsTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Bounds);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.BoundsField(fieldName, (Bounds)value);
        }
    }
}