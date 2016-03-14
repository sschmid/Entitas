using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class EnumTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type.IsEnum;
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.EnumPopup(memberName, (Enum)value);
        }
    }
}