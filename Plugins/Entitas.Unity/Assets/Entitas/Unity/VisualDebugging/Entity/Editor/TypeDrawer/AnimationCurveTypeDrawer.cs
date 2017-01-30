using System;
using Entitas.Api;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class AnimationCurveTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(AnimationCurve);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            return EditorGUILayout.CurveField(memberName, (AnimationCurve)value);
        }
    }
}
