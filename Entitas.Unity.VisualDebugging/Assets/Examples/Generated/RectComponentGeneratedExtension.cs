namespace Entitas {
    public partial class Entity {
        public RectComponent rect { get { return (RectComponent)GetComponent(ComponentIds.Rect); } }

        public bool hasRect { get { return HasComponent(ComponentIds.Rect); } }

        public Entity AddRect(UnityEngine.Rect newRect) {
            var componentPool = GetComponentPool(ComponentIds.Rect);
            var component = (RectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new RectComponent());
            component.rect = newRect;
            return AddComponent(ComponentIds.Rect, component);
        }

        public Entity ReplaceRect(UnityEngine.Rect newRect) {
            var componentPool = GetComponentPool(ComponentIds.Rect);
            var component = (RectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new RectComponent());
            component.rect = newRect;
            ReplaceComponent(ComponentIds.Rect, component);
            return this;
        }

        public Entity RemoveRect() {
            return RemoveComponent(ComponentIds.Rect);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherRect;

        public static IMatcher Rect {
            get {
                if (_matcherRect == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Rect);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherRect = matcher;
                }

                return _matcherRect;
            }
        }
    }
}
