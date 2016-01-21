namespace Entitas {
    public partial class Entity {
        static readonly ComponentB componentBComponent = new ComponentB();

        public bool isComponentB {
            get { return HasComponent(ComponentIds.ComponentB); }
            set {
                if (value != isComponentB) {
                    if (value) {
                        AddComponent(ComponentIds.ComponentB, componentBComponent);
                    } else {
                        RemoveComponent(ComponentIds.ComponentB);
                    }
                }
            }
        }

        public Entity IsComponentB(bool value) {
            isComponentB = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherComponentB;

        public static IMatcher ComponentB {
            get {
                if (_matcherComponentB == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.ComponentB);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherComponentB = matcher;
                }

                return _matcherComponentB;
            }
        }
    }
}
