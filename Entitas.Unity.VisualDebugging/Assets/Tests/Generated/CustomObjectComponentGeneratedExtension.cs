namespace Entitas {
    public partial class Entity {
        public CustomObjectComponent customObject { get { return (CustomObjectComponent)GetComponent(ComponentIds.CustomObject); } }

        public bool hasCustomObject { get { return HasComponent(ComponentIds.CustomObject); } }

        public void AddCustomObject(CustomObjectComponent component) {
            AddComponent(ComponentIds.CustomObject, component);
        }

        public void AddCustomObject(CustomObject newCustomObject) {
            var component = new CustomObjectComponent();
            component.customObject = newCustomObject;
            AddCustomObject(component);
        }

        public void ReplaceCustomObject(CustomObject newCustomObject) {
            CustomObjectComponent component;
            if (hasCustomObject) {
                WillRemoveComponent(ComponentIds.CustomObject);
                component = customObject;
            } else {
                component = new CustomObjectComponent();
            }
            component.customObject = newCustomObject;
            ReplaceComponent(ComponentIds.CustomObject, component);
        }

        public void RemoveCustomObject() {
            RemoveComponent(ComponentIds.CustomObject);
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
