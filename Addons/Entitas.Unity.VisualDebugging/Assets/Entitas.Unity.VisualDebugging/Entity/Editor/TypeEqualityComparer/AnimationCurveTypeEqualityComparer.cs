using System;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    // AnimationCurve.Equals only returns true if value and otherValue are the same reference
    public class AnimationCurveTypeEqualityComparer : ITypeEqualityComparer {

        public bool HandlesType(Type type) {
            return type == typeof(AnimationCurve);
        }

        public bool Equals(object x, object y) {
            if(x == y) {
                return true;
            }

            var animationCurve = (AnimationCurve)x;
            var otherAnimationCurve = (AnimationCurve)y;

            return animationCurve.length == otherAnimationCurve.length
               && animationCurve.preWrapMode == otherAnimationCurve.preWrapMode
               && animationCurve.postWrapMode == otherAnimationCurve.postWrapMode
               && ArrayUtility.ArrayEquals(animationCurve.keys, otherAnimationCurve.keys);
        }
    }
}
