public partial class TestEntity {

    public StandardComponent standard { get { return (StandardComponent)GetComponent(TestComponentsLookup.Standard); } }
    public bool hasStandard { get { return HasComponent(TestComponentsLookup.Standard); } }

    public void AddStandard(string newValue) {
        var component = CreateComponent<StandardComponent>(TestComponentsLookup.Standard);
        component.value = newValue;
        AddComponent(TestComponentsLookup.Standard, component);
    }

    public void ReplaceStandard(string newValue) {
        var component = CreateComponent<StandardComponent>(TestComponentsLookup.Standard);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.Standard, component);
    }

    public void RemoveStandard() {
        RemoveComponent(TestComponentsLookup.Standard);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherStandard;

    public static Entitas.IMatcher<TestEntity> Standard {
        get {
            if(_matcherStandard == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.Standard);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherStandard = matcher;
            }

            return _matcherStandard;
        }
    }
}
