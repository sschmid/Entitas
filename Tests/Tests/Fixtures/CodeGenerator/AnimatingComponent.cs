using Entitas.CodeGenerator;
using Entitas;

[SingleEntity]
public class AnimatingComponent : IComponent {
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
    }

    public partial class EntityRepository {
        public Entity animatingEntity { get { return GetCollection(Matcher.Animating).GetSingleEntity(); } }

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

    public static partial class Matcher {
        static AllOfEntityMatcher _matcherAnimating;

        public static AllOfEntityMatcher Animating {
            get {
                if (_matcherAnimating == null) {
                    _matcherAnimating = EntityMatcher.AllOf(new [] { ComponentIds.Animating });
                }

                return _matcherAnimating;
            }
        }
    }
}";
}
