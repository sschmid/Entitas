using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class Vector2TypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Vector2);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.Vector2Field(memberName, (Vector2)value);
        }
    }
}
