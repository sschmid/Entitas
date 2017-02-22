public partial class TestEntity {

    public NameAgeComponent nameAge { get { return (NameAgeComponent)GetComponent(TestComponentsLookup.NameAge); } }
    public bool hasNameAge { get { return HasComponent(TestComponentsLookup.NameAge); } }

    public void AddNameAge(string newName, int newAge) {
        var component = CreateComponent<NameAgeComponent>(TestComponentsLookup.NameAge);
        component.name = newName;
        component.age = newAge;
        AddComponent(TestComponentsLookup.NameAge, component);
    }

    public void ReplaceNameAge(string newName, int newAge) {
        var component = CreateComponent<NameAgeComponent>(TestComponentsLookup.NameAge);
        component.name = newName;
        component.age = newAge;
        ReplaceComponent(TestComponentsLookup.NameAge, component);
    }

    public void RemoveNameAge() {
        RemoveComponent(TestComponentsLookup.NameAge);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherNameAge;

    public static Entitas.IMatcher<TestEntity> NameAge {
        get {
            if(_matcherNameAge == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.NameAge);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherNameAge = matcher;
            }

            return _matcherNameAge;
        }
    }
}
