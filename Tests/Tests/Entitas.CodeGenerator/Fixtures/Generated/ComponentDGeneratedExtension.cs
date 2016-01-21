namespace Entitas {
    public partial class Entity {
        static readonly ComponentD componentDComponent = new ComponentD();

        public bool isComponentD {
            get { return HasComponent(ComponentIds.ComponentD); }
            set {
                if (value != isComponentD) {
                    if (value) {
                        AddComponent(ComponentIds.ComponentD, componentDComponent);
                    } else {
                        RemoveComponent(ComponentIds.ComponentD);
                    }
                }
            }
        }

        public Entity IsComponentD(bool value) {
            isComponentD = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherComponentD;

        public static IMatcher ComponentD {
            get {
                if (_matcherComponentD == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.ComponentD);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherComponentD = matcher;
                }

                return _matcherComponentD;
            }
        }
    }
}
