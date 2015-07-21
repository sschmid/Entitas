namespace Entitas {
    public partial class Entity {
        public PositionComponent position { get { return (PositionComponent)GetComponent(ComponentIds.Position); } }

        public bool hasPosition { get { return HasComponent(ComponentIds.Position); } }

        public Entity AddPosition(PositionComponent component) {
            return AddComponent(ComponentIds.Position, component);
        }

        public Entity AddPosition(int newX, int newY) {
            var component = new PositionComponent();
            component.x = newX;
            component.y = newY;
            return AddPosition(component);
        }

        public Entity ReplacePosition(int newX, int newY) {
            PositionComponent component;
            if (hasPosition) {
                component = position;
            } else {
                component = new PositionComponent();
            }
            component.x = newX;
            component.y = newY;
            return ReplaceComponent(ComponentIds.Position, component);
        }

        public Entity RemovePosition() {
            return RemoveComponent(ComponentIds.Position);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherPosition;

        public static AllOfMatcher Position {
            get {
                if (_matcherPosition == null) {
                    _matcherPosition = new Matcher(ComponentIds.Position);
                }

                return _matcherPosition;
            }
        }
    }
}
