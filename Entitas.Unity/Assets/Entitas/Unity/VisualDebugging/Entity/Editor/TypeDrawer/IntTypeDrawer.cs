using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class IntTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(int);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.IntField(memberName, (int)value);
        }
    }
}
