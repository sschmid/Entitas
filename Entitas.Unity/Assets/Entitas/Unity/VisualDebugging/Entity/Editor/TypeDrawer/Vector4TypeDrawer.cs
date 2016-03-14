using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class Vector4TypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Vector4);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.Vector4Field(memberName, (Vector4)value);
        }
    }
}