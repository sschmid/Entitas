using System;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class UnityObjectTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) =>
            type == typeof(UnityEngine.Object) ||
            type.IsSubclassOf(typeof(UnityEngine.Object));

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) => 
            EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true);
    }
}
