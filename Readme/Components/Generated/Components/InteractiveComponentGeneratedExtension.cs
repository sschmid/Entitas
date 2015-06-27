namespace Entitas {
    public partial class Entity {
        static readonly InteractiveComponent interactiveComponent = new InteractiveComponent();

        public bool isInteractive {
            get { return HasComponent(ComponentIds.Interactive); }
            set {
                if (value != isInteractive) {
                    if (value) {
                        AddComponent(ComponentIds.Interactive, interactiveComponent);
                    } else {
                        RemoveComponent(ComponentIds.Interactive);
                    }
                }
            }
        }

        public Entity IsInteractive(bool value) {
            isInteractive = value;
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherInteractive;

        public static AllOfMatcher Interactive {
            get {
                if (_matcherInteractive == null) {
                    _matcherInteractive = new Matcher(ComponentIds.Interactive);
                }

                return _matcherInteractive;
            }
        }
    }
}
