using System;
using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class AnimationCurveTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(AnimationCurve);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.CurveField(fieldName, (AnimationCurve)value);
        }
    }
}