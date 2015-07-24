namespace Entitas {
    public partial class Entity {
        public RectComponent rect { get { return (RectComponent)GetComponent(ComponentIds.Rect); } }

        public bool hasRect { get { return HasComponent(ComponentIds.Rect); } }

        public Entity AddRect(RectComponent component) {
            return AddComponent(ComponentIds.Rect, component);
        }

        public Entity AddRect(UnityEngine.Rect newRect) {
            var component = new RectComponent();
            component.rect = newRect;
            return AddRect(component);
        }

        public Entity ReplaceRect(UnityEngine.Rect newRect) {
            RectComponent component;
            if (hasRect) {
                component = rect;
            } else {
                component = new RectComponent();
            }
            component.rect = newRect;
            return ReplaceComponent(ComponentIds.Rect, component);
        }

        public Entity RemoveRect() {
            return RemoveComponent(ComponentIds.Rect);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherRect;

        public static AllOfMatcher Rect {
            get {
                if (_matcherRect == null) {
                    _matcherRect = new Matcher(ComponentIds.Rect);
                }

                return _matcherRect;
            }
        }
    }
}
