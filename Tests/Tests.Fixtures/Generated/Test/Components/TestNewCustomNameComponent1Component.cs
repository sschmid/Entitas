public partial class TestEntity {

    public NewCustomNameComponent1Component newCustomNameComponent1 { get { return (NewCustomNameComponent1Component)GetComponent(TestComponentsLookup.NewCustomNameComponent1); } }
    public bool hasNewCustomNameComponent1 { get { return HasComponent(TestComponentsLookup.NewCustomNameComponent1); } }

    public void AddNewCustomNameComponent1(CustomName newValue) {
        var component = CreateComponent<NewCustomNameComponent1Component>(TestComponentsLookup.NewCustomNameComponent1);
        component.value = newValue;
        AddComponent(TestComponentsLookup.NewCustomNameComponent1, component);
    }

    public void ReplaceNewCustomNameComponent1(CustomName newValue) {
        var component = CreateComponent<NewCustomNameComponent1Component>(TestComponentsLookup.NewCustomNameComponent1);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.NewCustomNameComponent1, component);
    }

    public void RemoveNewCustomNameComponent1() {
        RemoveComponent(TestComponentsLookup.NewCustomNameComponent1);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherNewCustomNameComponent1;

    public static Entitas.IMatcher<TestEntity> NewCustomNameComponent1 {
        get {
            if(_matcherNewCustomNameComponent1 == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.NewCustomNameComponent1);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherNewCustomNameComponent1 = matcher;
            }

            return _matcherNewCustomNameComponent1;
        }
    }
}
