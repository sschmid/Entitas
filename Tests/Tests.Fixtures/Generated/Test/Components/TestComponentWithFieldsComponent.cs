public partial class TestEntity {

    public ComponentWithFields componentWithFields { get { return (ComponentWithFields)GetComponent(TestComponentsLookup.ComponentWithFields); } }
    public bool hasComponentWithFields { get { return HasComponent(TestComponentsLookup.ComponentWithFields); } }

    public void AddComponentWithFields(string newPublicField) {
        var component = CreateComponent<ComponentWithFields>(TestComponentsLookup.ComponentWithFields);
        component.publicField = newPublicField;
        AddComponent(TestComponentsLookup.ComponentWithFields, component);
    }

    public void ReplaceComponentWithFields(string newPublicField) {
        var component = CreateComponent<ComponentWithFields>(TestComponentsLookup.ComponentWithFields);
        component.publicField = newPublicField;
        ReplaceComponent(TestComponentsLookup.ComponentWithFields, component);
    }

    public void RemoveComponentWithFields() {
        RemoveComponent(TestComponentsLookup.ComponentWithFields);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherComponentWithFields;

    public static Entitas.IMatcher<TestEntity> ComponentWithFields {
        get {
            if(_matcherComponentWithFields == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.ComponentWithFields);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherComponentWithFields = matcher;
            }

            return _matcherComponentWithFields;
        }
    }
}
