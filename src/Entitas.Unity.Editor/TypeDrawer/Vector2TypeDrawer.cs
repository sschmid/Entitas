using System;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class Vector2TypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type == typeof(Vector2);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) =>
            EditorGUILayout.Vector2Field(memberName, (Vector2)value);
    }
}
