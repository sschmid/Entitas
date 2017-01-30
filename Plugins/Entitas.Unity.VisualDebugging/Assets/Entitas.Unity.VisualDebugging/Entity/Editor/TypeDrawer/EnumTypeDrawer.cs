using System;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {

    public class EnumTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.IsEnum;
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            if(memberType.IsDefined(typeof(FlagsAttribute), false)) {
                return EditorGUILayout.EnumMaskField(memberName, (Enum)value);
            }
            return EditorGUILayout.EnumPopup(memberName, (Enum)value);
        }
    }
}
