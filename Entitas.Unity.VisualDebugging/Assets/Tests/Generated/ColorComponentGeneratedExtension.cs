namespace Entitas {
    public partial class Entity {
        public ColorComponent color { get { return (ColorComponent)GetComponent(ComponentIds.Color); } }

        public bool hasColor { get { return HasComponent(ComponentIds.Color); } }

        public void AddColor(ColorComponent component) {
            AddComponent(ComponentIds.Color, component);
        }

        public void AddColor(UnityEngine.Color newColor) {
            var component = new ColorComponent();
            component.color = newColor;
            AddColor(component);
        }

        public void ReplaceColor(UnityEngine.Color newColor) {
            ColorComponent component;
            if (hasColor) {
                WillRemoveComponent(ComponentIds.Color);
                component = color;
            } else {
                component = new ColorComponent();
            }
            component.color = newColor;
            ReplaceComponent(ComponentIds.Color, component);
        }

        public void RemoveColor() {
            RemoveComponent(ComponentIds.Color);
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
