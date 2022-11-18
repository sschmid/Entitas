using System;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class DoubleTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(double);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            EditorGUILayout.DoubleField(memberName, (double)value);
    }
}
