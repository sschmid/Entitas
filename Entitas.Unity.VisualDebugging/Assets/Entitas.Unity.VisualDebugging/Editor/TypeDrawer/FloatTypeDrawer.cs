using System;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class FloatTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(float);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.FloatField(fieldName, (float)value);
        }
    }
}