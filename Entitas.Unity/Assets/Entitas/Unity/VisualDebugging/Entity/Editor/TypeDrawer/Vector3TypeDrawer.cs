using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class Vector3TypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Vector3);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.Vector3Field(memberName, (Vector3)value);
        }
    }
}

