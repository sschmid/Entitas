using System;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class ColorTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(Color);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            EditorGUILayout.ColorField(memberName, (Color)value);
    }
}
