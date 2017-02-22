public partial class TestEntity {

    public ComponentWithProperties componentWithProperties { get { return (ComponentWithProperties)GetComponent(TestComponentsLookup.ComponentWithProperties); } }
    public bool hasComponentWithProperties { get { return HasComponent(TestComponentsLookup.ComponentWithProperties); } }

    public void AddComponentWithProperties(string newPublicProperty) {
        var component = CreateComponent<ComponentWithProperties>(TestComponentsLookup.ComponentWithProperties);
        component.publicProperty = newPublicProperty;
        AddComponent(TestComponentsLookup.ComponentWithProperties, component);
    }

    public void ReplaceComponentWithProperties(string newPublicProperty) {
        var component = CreateComponent<ComponentWithProperties>(TestComponentsLookup.ComponentWithProperties);
        component.publicProperty = newPublicProperty;
        ReplaceComponent(TestComponentsLookup.ComponentWithProperties, component);
    }

    public void RemoveComponentWithProperties() {
        RemoveComponent(TestComponentsLookup.ComponentWithProperties);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherComponentWithProperties;

    public static Entitas.IMatcher<TestEntity> ComponentWithProperties {
        get {
            if(_matcherComponentWithProperties == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.ComponentWithProperties);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherComponentWithProperties = matcher;
            }

            return _matcherComponentWithProperties;
        }
    }
}
