using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class BoundsTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Bounds);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.BoundsField(memberName, (Bounds)value);
        }
    }
}