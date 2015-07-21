namespace Entitas {
    public partial class Entity {
        public UnsupportedObjectComponent unsupportedObject { get { return (UnsupportedObjectComponent)GetComponent(ComponentIds.UnsupportedObject); } }

        public bool hasUnsupportedObject { get { return HasComponent(ComponentIds.UnsupportedObject); } }

        public Entity AddUnsupportedObject(UnsupportedObjectComponent component) {
            return AddComponent(ComponentIds.UnsupportedObject, component);
        }

        public Entity AddUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var component = new UnsupportedObjectComponent();
            component.unsupportedObject = newUnsupportedObject;
            return AddUnsupportedObject(component);
        }

        public Entity ReplaceUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            UnsupportedObjectComponent component;
            if (hasUnsupportedObject) {
                component = unsupportedObject;
            } else {
                component = new UnsupportedObjectComponent();
            }
            component.unsupportedObject = newUnsupportedObject;
            return ReplaceComponent(ComponentIds.UnsupportedObject, component);
        }

        public Entity RemoveUnsupportedObject() {
            return RemoveComponent(ComponentIds.UnsupportedObject);
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
