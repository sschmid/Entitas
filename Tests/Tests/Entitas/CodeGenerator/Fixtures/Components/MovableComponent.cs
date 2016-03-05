using Entitas;
using Entitas.CodeGenerator;

public class MovableComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(MovableComponent).ToCompilableString(),
                new ComponentFieldInfo[0],
                new string[0],
                false,
                "is",
                true,
                true
            );
        }
    }

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
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Movable);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMovable = matcher;
                }

                return _matcherMovable;
            }
        }
    }
}
";
}
