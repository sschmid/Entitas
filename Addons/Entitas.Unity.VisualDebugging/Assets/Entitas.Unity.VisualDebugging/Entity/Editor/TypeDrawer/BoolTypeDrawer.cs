using System;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {

    public class BoolTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(bool);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IComponent component) {
            return EditorGUILayout.Toggle(memberName, (bool)value);
        }
    }
}
