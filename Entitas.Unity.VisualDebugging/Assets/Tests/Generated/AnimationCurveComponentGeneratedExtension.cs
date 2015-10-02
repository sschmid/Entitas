using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public AnimationCurveComponent animationCurve { get { return (AnimationCurveComponent)GetComponent(ComponentIds.AnimationCurve); } }

        public bool hasAnimationCurve { get { return HasComponent(ComponentIds.AnimationCurve); } }

        static readonly Stack<AnimationCurveComponent> _animationCurveComponentPool = new Stack<AnimationCurveComponent>();

        public static void ClearAnimationCurveComponentPool() {
            _animationCurveComponentPool.Clear();
        }

        public Entity AddAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var component = _animationCurveComponentPool.Count > 0 ? _animationCurveComponentPool.Pop() : new AnimationCurveComponent();
            component.animationCurve = newAnimationCurve;
            return AddComponent(ComponentIds.AnimationCurve, component);
        }

        public Entity ReplaceAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var previousComponent = hasAnimationCurve ? animationCurve : null;
            var component = _animationCurveComponentPool.Count > 0 ? _animationCurveComponentPool.Pop() : new AnimationCurveComponent();
            component.animationCurve = newAnimationCurve;
            ReplaceComponent(ComponentIds.AnimationCurve, component);
            if (previousComponent != null) {
                _animationCurveComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveAnimationCurve() {
            var component = animationCurve;
            RemoveComponent(ComponentIds.AnimationCurve);
            _animationCurveComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherAnimationCurve;

        public static IMatcher AnimationCurve {
            get {
                if (_matcherAnimationCurve == null) {
                    _matcherAnimationCurve = Matcher.AllOf(ComponentIds.AnimationCurve);
                }

                return _matcherAnimationCurve;
            }
        }
    }
}
