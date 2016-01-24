namespace Entitas {
    public partial class Entity {
        static readonly ComponentE componentEComponent = new ComponentE();

        public bool isComponentE {
            get { return HasComponent(ComponentIds.ComponentE); }
            set {
                if (value != isComponentE) {
                    if (value) {
                        AddComponent(ComponentIds.ComponentE, componentEComponent);
                    } else {
                        RemoveComponent(ComponentIds.ComponentE);
                    }
                }
            }
        }

        public Entity IsComponentE(bool value) {
            isComponentE = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherComponentE;

        public static IMatcher ComponentE {
            get {
                if (_matcherComponentE == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.ComponentE);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherComponentE = matcher;
                }

                return _matcherComponentE;
            }
        }
    }
}
