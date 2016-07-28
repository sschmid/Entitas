using System;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class DoubleTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(double);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.DoubleField(memberName, (double)value);
        }
    }
}