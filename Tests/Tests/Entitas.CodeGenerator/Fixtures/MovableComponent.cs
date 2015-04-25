using Entitas;

public class MovableComponent : IComponent {
    public static string extensions =
        @"namespace Entitas {
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
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMovable;

        public static AllOfMatcher Movable {
            get {
                if (_matcherMovable == null) {
                    _matcherMovable = new Matcher(ComponentIds.Movable);
                }

                return _matcherMovable;
            }
        }
    }
}
";
}
