namespace Entitas {
    public partial class Entity {
        public MoveComponent move { get { return (MoveComponent)GetComponent(ComponentIds.Move); } }

        public bool hasMove { get { return HasComponent(ComponentIds.Move); } }

        public Entity AddMove(MoveComponent component) {
            return AddComponent(ComponentIds.Move, component);
        }

        public Entity AddMove(int newSpeed) {
            var component = new MoveComponent();
            component.speed = newSpeed;
            return AddMove(component);
        }

        public Entity ReplaceMove(int newSpeed) {
            MoveComponent component;
            if (hasMove) {
                WillRemoveComponent(ComponentIds.Move);
                component = move;
            } else {
                component = new MoveComponent();
            }
            component.speed = newSpeed;
            return ReplaceComponent(ComponentIds.Move, component);
        }

        public Entity RemoveMove() {
            return RemoveComponent(ComponentIds.Move);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMove;

        public static AllOfMatcher Move {
            get {
                if (_matcherMove == null) {
                    _matcherMove = new Matcher(ComponentIds.Move);
                }

                return _matcherMove;
            }
        }
    }
}
