namespace Entitas {
    public partial class Entity {
        public AnimationCurveComponent animationCurve { get { return (AnimationCurveComponent)GetComponent(ComponentIds.AnimationCurve); } }

        public bool hasAnimationCurve { get { return HasComponent(ComponentIds.AnimationCurve); } }

        public Entity AddAnimationCurve(AnimationCurveComponent component) {
            return AddComponent(ComponentIds.AnimationCurve, component);
        }

        public Entity AddAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var component = new AnimationCurveComponent();
            component.animationCurve = newAnimationCurve;
            return AddAnimationCurve(component);
        }

        public Entity ReplaceAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            AnimationCurveComponent component;
            if (hasAnimationCurve) {
                WillRemoveComponent(ComponentIds.AnimationCurve);
                component = animationCurve;
            } else {
                component = new AnimationCurveComponent();
            }
            component.animationCurve = newAnimationCurve;
            return ReplaceComponent(ComponentIds.AnimationCurve, component);
        }

        public Entity RemoveAnimationCurve() {
            return RemoveComponent(ComponentIds.AnimationCurve);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherAnimationCurve;

        public static AllOfMatcher AnimationCurve {
            get {
                if (_matcherAnimationCurve == null) {
                    _matcherAnimationCurve = new Matcher(ComponentIds.AnimationCurve);
                }

                return _matcherAnimationCurve;
            }
        }
    }
}
