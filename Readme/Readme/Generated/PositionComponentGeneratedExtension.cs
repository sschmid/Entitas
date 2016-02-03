namespace Entitas {
    public partial class Entity {
        public PositionComponent position { get { return (PositionComponent)GetComponent(ComponentIds.Position); } }

        public bool hasPosition { get { return HasComponent(ComponentIds.Position); } }

        public Entity AddPosition(int newX, int newY) {
            var componentPool = GetComponentPool(ComponentIds.Position);
            var component = (PositionComponent)(componentPool.Count > 0 ? componentPool.Pop() : new PositionComponent());
            component.x = newX;
            component.y = newY;
            return AddComponent(ComponentIds.Position, component);
        }

        public Entity ReplacePosition(int newX, int newY) {
            var componentPool = GetComponentPool(ComponentIds.Position);
            var component = (PositionComponent)(componentPool.Count > 0 ? componentPool.Pop() : new PositionComponent());
            component.x = newX;
            component.y = newY;
            ReplaceComponent(ComponentIds.Position, component);
            return this;
        }

        public Entity RemovePosition() {
            return RemoveComponent(ComponentIds.Position);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherPosition;

        public static IMatcher Position {
            get {
                if (_matcherPosition == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Position);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherPosition = matcher;
                }

                return _matcherPosition;
            }
        }
    }
}
