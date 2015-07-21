namespace Entitas {
    public partial class Entity {
        public CustomObjectComponent customObject { get { return (CustomObjectComponent)GetComponent(ComponentIds.CustomObject); } }

        public bool hasCustomObject { get { return HasComponent(ComponentIds.CustomObject); } }

        public Entity AddCustomObject(CustomObjectComponent component) {
            return AddComponent(ComponentIds.CustomObject, component);
        }

        public Entity AddCustomObject(CustomObject newCustomObject) {
            var component = new CustomObjectComponent();
            component.customObject = newCustomObject;
            return AddCustomObject(component);
        }

        public Entity ReplaceCustomObject(CustomObject newCustomObject) {
            CustomObjectComponent component;
            if (hasCustomObject) {
                component = customObject;
            } else {
                component = new CustomObjectComponent();
            }
            component.customObject = newCustomObject;
            return ReplaceComponent(ComponentIds.CustomObject, component);
        }

        public Entity RemoveCustomObject() {
            return RemoveComponent(ComponentIds.CustomObject);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherCustomObject;

        public static AllOfMatcher CustomObject {
            get {
                if (_matcherCustomObject == null) {
                    _matcherCustomObject = new Matcher(ComponentIds.CustomObject);
                }

                return _matcherCustomObject;
            }
        }
    }
}
