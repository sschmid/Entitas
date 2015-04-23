namespace Entitas {
    public partial class Entity {
        public BoundsComponent bounds { get { return (BoundsComponent)GetComponent(ComponentIds.Bounds); } }

        public bool hasBounds { get { return HasComponent(ComponentIds.Bounds); } }

        public void AddBounds(BoundsComponent component) {
            AddComponent(ComponentIds.Bounds, component);
        }

        public void AddBounds(UnityEngine.Bounds newBounds) {
            var component = new BoundsComponent();
            component.bounds = newBounds;
            AddBounds(component);
        }

        public void ReplaceBounds(UnityEngine.Bounds newBounds) {
            BoundsComponent component;
            if (hasBounds) {
                WillRemoveComponent(ComponentIds.Bounds);
                component = bounds;
            } else {
                component = new BoundsComponent();
            }
            component.bounds = newBounds;
            ReplaceComponent(ComponentIds.Bounds, component);
        }

        public void RemoveBounds() {
            RemoveComponent(ComponentIds.Bounds);
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
