using System;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public class EnumTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type.IsEnum;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            memberType.IsDefined(typeof(FlagsAttribute), false)
                ? EditorGUILayout.EnumFlagsField(memberName, (Enum)value)
                : EditorGUILayout.EnumPopup(memberName, (Enum)value);
    }
}
