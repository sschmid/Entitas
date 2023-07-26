using System;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class BoolTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(bool);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) => 
            EditorGUILayout.Toggle(memberName, (bool)value);
    }
}
