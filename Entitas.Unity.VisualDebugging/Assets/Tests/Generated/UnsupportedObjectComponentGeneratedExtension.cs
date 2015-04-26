namespace Entitas {
    public partial class Entity {
        public UnsupportedObjectComponent unsupportedObject { get { return (UnsupportedObjectComponent)GetComponent(ComponentIds.UnsupportedObject); } }

        public bool hasUnsupportedObject { get { return HasComponent(ComponentIds.UnsupportedObject); } }

        public void AddUnsupportedObject(UnsupportedObjectComponent component) {
            AddComponent(ComponentIds.UnsupportedObject, component);
        }

        public void AddUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var component = new UnsupportedObjectComponent();
            component.unsupportedObject = newUnsupportedObject;
            AddUnsupportedObject(component);
        }

        public void ReplaceUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            UnsupportedObjectComponent component;
            if (hasUnsupportedObject) {
                WillRemoveComponent(ComponentIds.UnsupportedObject);
                component = unsupportedObject;
            } else {
                component = new UnsupportedObjectComponent();
            }
            component.unsupportedObject = newUnsupportedObject;
            ReplaceComponent(ComponentIds.UnsupportedObject, component);
        }

        public void RemoveUnsupportedObject() {
            RemoveComponent(ComponentIds.UnsupportedObject);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherUnsupportedObject;

        public static AllOfMatcher UnsupportedObject {
            get {
                if (_matcherUnsupportedObject == null) {
                    _matcherUnsupportedObject = new Matcher(ComponentIds.UnsupportedObject);
                }

                return _matcherUnsupportedObject;
            }
        }
    }
}
