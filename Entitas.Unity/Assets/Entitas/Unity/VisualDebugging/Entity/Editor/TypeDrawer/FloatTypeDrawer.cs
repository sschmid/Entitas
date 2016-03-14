using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class FloatTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(float);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.FloatField(memberName, (float)value);
        }
    }
}