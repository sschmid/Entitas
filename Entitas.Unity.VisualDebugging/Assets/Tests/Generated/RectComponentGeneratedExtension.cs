using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public RectComponent rect { get { return (RectComponent)GetComponent(ComponentIds.Rect); } }

        public bool hasRect { get { return HasComponent(ComponentIds.Rect); } }

        static readonly Stack<RectComponent> _rectComponentPool = new Stack<RectComponent>();

        public static void ClearRectComponentPool() {
            _rectComponentPool.Clear();
        }

        public Entity AddRect(UnityEngine.Rect newRect) {
            var component = _rectComponentPool.Count > 0 ? _rectComponentPool.Pop() : new RectComponent();
            component.rect = newRect;
            return AddComponent(ComponentIds.Rect, component);
        }

        public Entity ReplaceRect(UnityEngine.Rect newRect) {
            var previousComponent = hasRect ? rect : null;
            var component = _rectComponentPool.Count > 0 ? _rectComponentPool.Pop() : new RectComponent();
            component.rect = newRect;
            ReplaceComponent(ComponentIds.Rect, component);
            if (previousComponent != null) {
                _rectComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveRect() {
            var component = rect;
            RemoveComponent(ComponentIds.Rect);
            _rectComponentPool.Push(component);
            return this;
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
