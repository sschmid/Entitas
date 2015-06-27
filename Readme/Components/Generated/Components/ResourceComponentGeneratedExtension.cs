namespace Entitas {
    public partial class Entity {
        public ResourceComponent resource { get { return (ResourceComponent)GetComponent(ComponentIds.Resource); } }

        public bool hasResource { get { return HasComponent(ComponentIds.Resource); } }

        public Entity AddResource(ResourceComponent component) {
            return AddComponent(ComponentIds.Resource, component);
        }

        public Entity AddResource(string newName) {
            var component = new ResourceComponent();
            component.name = newName;
            return AddResource(component);
        }

        public Entity ReplaceResource(string newName) {
            ResourceComponent component;
            if (hasResource) {
                WillRemoveComponent(ComponentIds.Resource);
                component = resource;
            } else {
                component = new ResourceComponent();
            }
            component.name = newName;
            return ReplaceComponent(ComponentIds.Resource, component);
        }

        public Entity RemoveResource() {
            return RemoveComponent(ComponentIds.Resource);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherResource;

        public static AllOfMatcher Resource {
            get {
                if (_matcherResource == null) {
                    _matcherResource = new Matcher(ComponentIds.Resource);
                }

                return _matcherResource;
            }
        }
    }
}
