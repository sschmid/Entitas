using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public ColorComponent color { get { return (ColorComponent)GetComponent(ComponentIds.Color); } }

        public bool hasColor { get { return HasComponent(ComponentIds.Color); } }

        static readonly Stack<ColorComponent> _colorComponentPool = new Stack<ColorComponent>();

        public static void ClearColorComponentPool() {
            _colorComponentPool.Clear();
        }

        public Entity AddColor(UnityEngine.Color newColor) {
            var component = _colorComponentPool.Count > 0 ? _colorComponentPool.Pop() : new ColorComponent();
            component.color = newColor;
            return AddComponent(ComponentIds.Color, component);
        }

        public Entity ReplaceColor(UnityEngine.Color newColor) {
            var previousComponent = color;
            var component = _colorComponentPool.Count > 0 ? _colorComponentPool.Pop() : new ColorComponent();
            component.color = newColor;
            ReplaceComponent(ComponentIds.Color, component);
            if (previousComponent != null) {
                _colorComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveColor() {
            var component = color;
            RemoveComponent(ComponentIds.Color);
            _colorComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherColor;

        public static AllOfMatcher Color {
            get {
                if (_matcherColor == null) {
                    _matcherColor = new Matcher(ComponentIds.Color);
                }

                return _matcherColor;
            }
        }
    }
}
