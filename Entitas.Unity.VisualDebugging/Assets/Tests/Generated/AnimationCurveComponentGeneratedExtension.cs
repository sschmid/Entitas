namespace Entitas {
    public partial class Entity {
        public AnimationCurveComponent animationCurve { get { return (AnimationCurveComponent)GetComponent(ComponentIds.AnimationCurve); } }

        public bool hasAnimationCurve { get { return HasComponent(ComponentIds.AnimationCurve); } }

        public void AddAnimationCurve(AnimationCurveComponent component) {
            AddComponent(ComponentIds.AnimationCurve, component);
        }

        public void AddAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var component = new AnimationCurveComponent();
            component.animationCurve = newAnimationCurve;
            AddAnimationCurve(component);
        }

        public void ReplaceAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            AnimationCurveComponent component;
            if (hasAnimationCurve) {
                WillRemoveComponent(ComponentIds.AnimationCurve);
                component = animationCurve;
            } else {
                component = new AnimationCurveComponent();
            }
            component.animationCurve = newAnimationCurve;
            ReplaceComponent(ComponentIds.AnimationCurve, component);
        }

        public void RemoveAnimationCurve() {
            RemoveComponent(ComponentIds.AnimationCurve);
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
