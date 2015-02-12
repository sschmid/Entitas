namespace Entitas {
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

    public static partial class Matcher {
        static AllOfMatcher _matcherAnimating;

        public static AllOfMatcher Animating {
            get {
                if (_matcherAnimating == null) {
                    _matcherAnimating = Matcher.AllOf(new [] { ComponentIds.Animating });
                }

                return _matcherAnimating;
            }
        }
    }
}