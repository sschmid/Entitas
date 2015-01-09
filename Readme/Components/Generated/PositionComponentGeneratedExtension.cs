namespace Entitas {
    public partial class Entity {
        public PositionComponent position { get { return (PositionComponent)GetComponent(ComponentIds.Position); } }

        public bool hasPosition { get { return HasComponent(ComponentIds.Position); } }

        public void AddPosition(PositionComponent component) {
            AddComponent(ComponentIds.Position, component);
        }

        public void AddPosition(int newX, int newY, int newZ) {
            var component = new PositionComponent();
            component.x = newX;
            component.y = newY;
            component.z = newZ;
            AddPosition(component);
        }

        public void ReplacePosition(int newX, int newY, int newZ) {
            PositionComponent component;
            if (hasPosition) {
                WillRemoveComponent(ComponentIds.Position);
                component = position;
            } else {
                component = new PositionComponent();
            }
            component.x = newX;
            component.y = newY;
            component.z = newZ;
            ReplaceComponent(ComponentIds.Position, component);
        }

        public void RemovePosition() {
            RemoveComponent(ComponentIds.Position);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherPosition;

        public static AllOfMatcher Position {
            get {
                if (_matcherPosition == null) {
                    _matcherPosition = Matcher.AllOf(new [] { ComponentIds.Position });
                }

                return _matcherPosition;
            }
        }
    }
}