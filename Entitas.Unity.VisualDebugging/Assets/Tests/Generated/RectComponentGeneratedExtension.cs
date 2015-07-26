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
            var previousComponent = rect;
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
