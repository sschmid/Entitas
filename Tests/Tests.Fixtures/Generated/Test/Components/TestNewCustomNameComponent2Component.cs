public partial class TestEntity {

    public NewCustomNameComponent2Component newCustomNameComponent2 { get { return (NewCustomNameComponent2Component)GetComponent(TestComponentsLookup.NewCustomNameComponent2); } }
    public bool hasNewCustomNameComponent2 { get { return HasComponent(TestComponentsLookup.NewCustomNameComponent2); } }

    public void AddNewCustomNameComponent2(CustomName newValue) {
        var component = CreateComponent<NewCustomNameComponent2Component>(TestComponentsLookup.NewCustomNameComponent2);
        component.value = newValue;
        AddComponent(TestComponentsLookup.NewCustomNameComponent2, component);
    }

    public void ReplaceNewCustomNameComponent2(CustomName newValue) {
        var component = CreateComponent<NewCustomNameComponent2Component>(TestComponentsLookup.NewCustomNameComponent2);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.NewCustomNameComponent2, component);
    }

    public void RemoveNewCustomNameComponent2() {
        RemoveComponent(TestComponentsLookup.NewCustomNameComponent2);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherNewCustomNameComponent2;

    public static Entitas.IMatcher<TestEntity> NewCustomNameComponent2 {
        get {
            if(_matcherNewCustomNameComponent2 == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.NewCustomNameComponent2);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherNewCustomNameComponent2 = matcher;
            }

            return _matcherNewCustomNameComponent2;
        }
    }
}
