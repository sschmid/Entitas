using System;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class StringTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(string);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.TextField(fieldName, (string)value);
        }
    }
}