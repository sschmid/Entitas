namespace Entitas {
    public partial class Entity {
        public SystemObjectComponent systemObject { get { return (SystemObjectComponent)GetComponent(ComponentIds.SystemObject); } }

        public bool hasSystemObject { get { return HasComponent(ComponentIds.SystemObject); } }

        public Entity AddSystemObject(SystemObjectComponent component) {
            return AddComponent(ComponentIds.SystemObject, component);
        }

        public Entity AddSystemObject(object newSystemObject) {
            var component = new SystemObjectComponent();
            component.systemObject = newSystemObject;
            return AddSystemObject(component);
        }

        public Entity ReplaceSystemObject(object newSystemObject) {
            SystemObjectComponent component;
            if (hasSystemObject) {
                component = systemObject;
            } else {
                component = new SystemObjectComponent();
            }
            component.systemObject = newSystemObject;
            return ReplaceComponent(ComponentIds.SystemObject, component);
        }

        public Entity RemoveSystemObject() {
            return RemoveComponent(ComponentIds.SystemObject);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherSystemObject;

        public static AllOfMatcher SystemObject {
            get {
                if (_matcherSystemObject == null) {
                    _matcherSystemObject = new Matcher(ComponentIds.SystemObject);
                }

                return _matcherSystemObject;
            }
        }
    }
}
