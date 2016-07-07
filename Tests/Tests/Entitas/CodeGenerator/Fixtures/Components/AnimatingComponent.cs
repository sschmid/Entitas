using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[SingleEntity]
public class AnimatingComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(AnimatingComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new string[0],
                true,
                "is",
                false,
                true,
                true
            );
        }
    }

    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        static readonly AnimatingComponent animatingComponent = new AnimatingComponent();

        public bool isAnimating {
            get { return HasComponent(ComponentIds.Animating); }
            set {
                if (value != isAnimating) {
                    if (value) {
                        AddComponent(ComponentIds.Animating, animatingComponent);
                    } else {
                        RemoveComponent(ComponentIds.Animating);
                    }
                }
            }
        }

        public Entity IsAnimating(bool value) {
            isAnimating = value;
            return this;
        }
    }

    public partial class Pool {
        public Entity animatingEntity { get { return GetGroup(Matcher.Animating).GetSingleEntity(); } }

        public bool isAnimating {
            get { return animatingEntity != null; }
            set {
                var entity = animatingEntity;
                if (value != (entity != null)) {
                    if (value) {
                        CreateEntity().isAnimating = true;
                    } else {
                        DestroyEntity(entity);
                    }
                }
            }
        }
    }

    public partial class Matcher {
        static IMatcher _matcherAnimating;

        public static IMatcher Animating {
            get {
                if (_matcherAnimating == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Animating);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherAnimating = matcher;
                }

                return _matcherAnimating;
            }
        }
    }
}
";
}
