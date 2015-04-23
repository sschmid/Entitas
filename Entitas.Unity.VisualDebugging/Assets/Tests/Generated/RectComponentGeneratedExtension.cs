namespace Entitas {
    public partial class Entity {
        public RectComponent rect { get { return (RectComponent)GetComponent(ComponentIds.Rect); } }

        public bool hasRect { get { return HasComponent(ComponentIds.Rect); } }

        public void AddRect(RectComponent component) {
            AddComponent(ComponentIds.Rect, component);
        }

        public void AddRect(UnityEngine.Rect newRect) {
            var component = new RectComponent();
            component.rect = newRect;
            AddRect(component);
        }

        public void ReplaceRect(UnityEngine.Rect newRect) {
            RectComponent component;
            if (hasRect) {
                WillRemoveComponent(ComponentIds.Rect);
                component = rect;
            } else {
                component = new RectComponent();
            }
            component.rect = newRect;
            ReplaceComponent(ComponentIds.Rect, component);
        }

        public void RemoveRect() {
            RemoveComponent(ComponentIds.Rect);
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
