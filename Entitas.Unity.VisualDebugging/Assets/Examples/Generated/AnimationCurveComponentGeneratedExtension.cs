namespace Entitas {
    public partial class Entity {
        public AnimationCurveComponent animationCurve { get { return (AnimationCurveComponent)GetComponent(ComponentIds.AnimationCurve); } }

        public bool hasAnimationCurve { get { return HasComponent(ComponentIds.AnimationCurve); } }

        public Entity AddAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var componentPool = GetComponentPool(ComponentIds.AnimationCurve);
            var component = (AnimationCurveComponent)(componentPool.Count > 0 ? componentPool.Pop() : new AnimationCurveComponent());
            component.animationCurve = newAnimationCurve;
            return AddComponent(ComponentIds.AnimationCurve, component);
        }

        public Entity ReplaceAnimationCurve(UnityEngine.AnimationCurve newAnimationCurve) {
            var componentPool = GetComponentPool(ComponentIds.AnimationCurve);
            var component = (AnimationCurveComponent)(componentPool.Count > 0 ? componentPool.Pop() : new AnimationCurveComponent());
            component.animationCurve = newAnimationCurve;
            ReplaceComponent(ComponentIds.AnimationCurve, component);
            return this;
        }

        public Entity RemoveAnimationCurve() {
            return RemoveComponent(ComponentIds.AnimationCurve);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherAnimationCurve;

        public static IMatcher AnimationCurve {
            get {
                if (_matcherAnimationCurve == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.AnimationCurve);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherAnimationCurve = matcher;
                }

                return _matcherAnimationCurve;
            }
        }
    }
}
