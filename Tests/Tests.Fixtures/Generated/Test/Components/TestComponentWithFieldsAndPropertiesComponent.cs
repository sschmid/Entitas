public partial class TestEntity {

    public ComponentWithFieldsAndProperties componentWithFieldsAndProperties { get { return (ComponentWithFieldsAndProperties)GetComponent(TestComponentsLookup.ComponentWithFieldsAndProperties); } }
    public bool hasComponentWithFieldsAndProperties { get { return HasComponent(TestComponentsLookup.ComponentWithFieldsAndProperties); } }

    public void AddComponentWithFieldsAndProperties(string newPublicField, string newPublicProperty) {
        var component = CreateComponent<ComponentWithFieldsAndProperties>(TestComponentsLookup.ComponentWithFieldsAndProperties);
        component.publicField = newPublicField;
        component.publicProperty = newPublicProperty;
        AddComponent(TestComponentsLookup.ComponentWithFieldsAndProperties, component);
    }

    public void ReplaceComponentWithFieldsAndProperties(string newPublicField, string newPublicProperty) {
        var component = CreateComponent<ComponentWithFieldsAndProperties>(TestComponentsLookup.ComponentWithFieldsAndProperties);
        component.publicField = newPublicField;
        component.publicProperty = newPublicProperty;
        ReplaceComponent(TestComponentsLookup.ComponentWithFieldsAndProperties, component);
    }

    public void RemoveComponentWithFieldsAndProperties() {
        RemoveComponent(TestComponentsLookup.ComponentWithFieldsAndProperties);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherComponentWithFieldsAndProperties;

    public static Entitas.IMatcher<TestEntity> ComponentWithFieldsAndProperties {
        get {
            if(_matcherComponentWithFieldsAndProperties == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.ComponentWithFieldsAndProperties);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherComponentWithFieldsAndProperties = matcher;
            }

            return _matcherComponentWithFieldsAndProperties;
        }
    }
}
