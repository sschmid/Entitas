using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class RectTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(Rect);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.RectField(memberName, (Rect)value);
        }
    }
}
