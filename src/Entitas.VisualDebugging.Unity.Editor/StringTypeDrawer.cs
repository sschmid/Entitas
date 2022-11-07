using System;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public class StringTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(string);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            EditorGUILayout.DelayedTextField(memberName, (string)value);
    }
}
