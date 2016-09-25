namespace Entitas {
    public partial class Entity {
        public ComponentWithFieldsAndProperties componentWithFieldsAndProperties { get { return (ComponentWithFieldsAndProperties)GetComponent(ComponentIds.ComponentWithFieldsAndProperties); } }

        public bool hasComponentWithFieldsAndProperties { get { return HasComponent(ComponentIds.ComponentWithFieldsAndProperties); } }

        public Entity AddComponentWithFieldsAndProperties(string newPublicField, string newPublicProperty) {
            var component = CreateComponent<ComponentWithFieldsAndProperties>(ComponentIds.ComponentWithFieldsAndProperties);
            component.publicField = newPublicField;
            component.publicProperty = newPublicProperty;
            return AddComponent(ComponentIds.ComponentWithFieldsAndProperties, component);
        }

        public Entity ReplaceComponentWithFieldsAndProperties(string newPublicField, string newPublicProperty) {
            var component = CreateComponent<ComponentWithFieldsAndProperties>(ComponentIds.ComponentWithFieldsAndProperties);
            component.publicField = newPublicField;
            component.publicProperty = newPublicProperty;
            ReplaceComponent(ComponentIds.ComponentWithFieldsAndProperties, component);
            return this;
        }

        public Entity RemoveComponentWithFieldsAndProperties() {
            return RemoveComponent(ComponentIds.ComponentWithFieldsAndProperties);
        }
    }

    public partial class Matcher {
        static IMatcher _matcherComponentWithFieldsAndProperties;

        public static IMatcher ComponentWithFieldsAndProperties {
            get {
                if(_matcherComponentWithFieldsAndProperties == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.ComponentWithFieldsAndProperties);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherComponentWithFieldsAndProperties = matcher;
                }

                return _matcherComponentWithFieldsAndProperties;
            }
        }
    }
}
