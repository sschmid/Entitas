namespace Entitas {
    public partial class Entity {
        public BoundsComponent bounds { get { return (BoundsComponent)GetComponent(ComponentIds.Bounds); } }

        public bool hasBounds { get { return HasComponent(ComponentIds.Bounds); } }

        public Entity AddBounds(BoundsComponent component) {
            return AddComponent(ComponentIds.Bounds, component);
        }

        public Entity AddBounds(UnityEngine.Bounds newBounds) {
            var component = new BoundsComponent();
            component.bounds = newBounds;
            return AddBounds(component);
        }

        public Entity ReplaceBounds(UnityEngine.Bounds newBounds) {
            BoundsComponent component;
            if (hasBounds) {
                component = bounds;
            } else {
                component = new BoundsComponent();
            }
            component.bounds = newBounds;
            return ReplaceComponent(ComponentIds.Bounds, component);
        }

        public Entity RemoveBounds() {
            return RemoveComponent(ComponentIds.Bounds);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherBounds;

        public static AllOfMatcher Bounds {
            get {
                if (_matcherBounds == null) {
                    _matcherBounds = new Matcher(ComponentIds.Bounds);
                }

                return _matcherBounds;
            }
        }
    }
}
