using Entitas;
using Entitas.CodeGenerator;

#pragma warning disable
public class ComponentWithFieldsAndProperties : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(ComponentWithFieldsAndProperties) })[0];
        }
    }

    // Has one public field

    public string publicField;
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;

    // Has one public property

    public string publicProperty { get; set; }
    public static bool publicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }

    public static string extensions =
        @"namespace Entitas {
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
                if (_matcherComponentWithFieldsAndProperties == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.ComponentWithFieldsAndProperties);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherComponentWithFieldsAndProperties = matcher;
                }

                return _matcherComponentWithFieldsAndProperties;
            }
        }
    }
}
";
}
