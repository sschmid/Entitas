namespace Entitas {
    public partial class Entity {
        public ColorComponent color { get { return (ColorComponent)GetComponent(ComponentIds.Color); } }

        public bool hasColor { get { return HasComponent(ComponentIds.Color); } }

        public Entity AddColor(ColorComponent component) {
            return AddComponent(ComponentIds.Color, component);
        }

        public Entity AddColor(UnityEngine.Color newColor) {
            var component = new ColorComponent();
            component.color = newColor;
            return AddColor(component);
        }

        public Entity ReplaceColor(UnityEngine.Color newColor) {
            ColorComponent component;
            if (hasColor) {
                WillRemoveComponent(ComponentIds.Color);
                component = color;
            } else {
                component = new ColorComponent();
            }
            component.color = newColor;
            return ReplaceComponent(ComponentIds.Color, component);
        }

        public Entity RemoveColor() {
            return RemoveComponent(ComponentIds.Color);
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
