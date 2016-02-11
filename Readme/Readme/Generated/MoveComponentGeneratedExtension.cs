namespace Entitas {
    public partial class Entity {
        public MoveComponent move { get { return (MoveComponent)GetComponent(ComponentIds.Move); } }

        public bool hasMove { get { return HasComponent(ComponentIds.Move); } }

        public Entity AddMove(int newSpeed) {
            var componentPool = GetComponentPool(ComponentIds.Move);
            var component = (MoveComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MoveComponent());
            component.speed = newSpeed;
            return AddComponent(ComponentIds.Move, component);
        }

        public Entity ReplaceMove(int newSpeed) {
            var componentPool = GetComponentPool(ComponentIds.Move);
            var component = (MoveComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MoveComponent());
            component.speed = newSpeed;
            ReplaceComponent(ComponentIds.Move, component);
            return this;
        }

        public Entity RemoveMove() {
            return RemoveComponent(ComponentIds.Move);
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMove;

        public static IMatcher Move {
            get {
                if (_matcherMove == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Move);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMove = matcher;
                }

                return _matcherMove;
            }
        }
    }
}
