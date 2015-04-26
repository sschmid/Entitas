using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class BoolTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(bool);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.Toggle(fieldName, (bool)value);
        }
    }
}