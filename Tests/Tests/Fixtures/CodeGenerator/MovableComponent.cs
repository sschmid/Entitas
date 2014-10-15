using Entitas;

public class MovableComponent : IComponent {
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        static readonly MovableComponent movableComponent = new MovableComponent();

        public bool isMovable { get { return HasComponent(ComponentIds.Movable); } }

        public void FlagMovable() {
            AddComponent(ComponentIds.Movable, movableComponent);
        }

        public void UnflagMovable() {
            RemoveComponent(ComponentIds.Movable);
        }
    }

    public static partial class Matcher {
        static AllOfEntityMatcher _matcherMovable;

        public static AllOfEntityMatcher Movable {
            get {
                if (_matcherMovable == null) {
                    _matcherMovable = EntityMatcher.AllOf(new [] { ComponentIds.Movable });
                }

                return _matcherMovable;
            }
        }
    }
}";
}
