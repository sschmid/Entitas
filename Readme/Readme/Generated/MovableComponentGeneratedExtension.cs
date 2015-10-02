namespace Entitas {
    public partial class Entity {
        static readonly MovableComponent movableComponent = new MovableComponent();

        public bool isMovable {
            get { return HasComponent(ComponentIds.Movable); }
            set {
                if (value != isMovable) {
                    if (value) {
                        AddComponent(ComponentIds.Movable, movableComponent);
                    } else {
                        RemoveComponent(ComponentIds.Movable);
                    }
                }
            }
        }

        public Entity IsMovable(bool value) {
            isMovable = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMovable;

        public static IMatcher Movable {
            get {
                if (_matcherMovable == null) {
                    _matcherMovable = Matcher.AllOf(ComponentIds.Movable);
                }

                return _matcherMovable;
            }
        }
    }
}
