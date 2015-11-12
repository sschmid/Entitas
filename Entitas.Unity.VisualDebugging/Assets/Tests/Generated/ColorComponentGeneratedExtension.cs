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
            var previousComponent = hasColor ? color : null;
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
        static IMatcher _matcherColor;

        public static IMatcher Color {
            get {
                if (_matcherColor == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Color);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherColor = matcher;
                }

                return _matcherColor;
            }
        }
    }
}
