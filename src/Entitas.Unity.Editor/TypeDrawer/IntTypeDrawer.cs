using System;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class IntTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(int);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            EditorGUILayout.IntField(memberName, (int)value);
    }
}
