namespace Entitas {
    public partial class Entity {
        public SystemObjectComponent systemObject { get { return (SystemObjectComponent)GetComponent(ComponentIds.SystemObject); } }

        public bool hasSystemObject { get { return HasComponent(ComponentIds.SystemObject); } }

        public void AddSystemObject(SystemObjectComponent component) {
            AddComponent(ComponentIds.SystemObject, component);
        }

        public void AddSystemObject(object newSystemObject) {
            var component = new SystemObjectComponent();
            component.systemObject = newSystemObject;
            AddSystemObject(component);
        }

        public void ReplaceSystemObject(object newSystemObject) {
            SystemObjectComponent component;
            if (hasSystemObject) {
                WillRemoveComponent(ComponentIds.SystemObject);
                component = systemObject;
            } else {
                component = new SystemObjectComponent();
            }
            component.systemObject = newSystemObject;
            ReplaceComponent(ComponentIds.SystemObject, component);
        }

        public void RemoveSystemObject() {
            RemoveComponent(ComponentIds.SystemObject);
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
