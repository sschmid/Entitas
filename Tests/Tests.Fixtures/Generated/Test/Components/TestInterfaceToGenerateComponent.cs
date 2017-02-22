public partial class TestEntity {

    public InterfaceToGenerateComponent interfaceToGenerate { get { return (InterfaceToGenerateComponent)GetComponent(TestComponentsLookup.InterfaceToGenerate); } }
    public bool hasInterfaceToGenerate { get { return HasComponent(TestComponentsLookup.InterfaceToGenerate); } }

    public void AddInterfaceToGenerate(My.Namespace.InterfaceToGenerate newValue) {
        var component = CreateComponent<InterfaceToGenerateComponent>(TestComponentsLookup.InterfaceToGenerate);
        component.value = newValue;
        AddComponent(TestComponentsLookup.InterfaceToGenerate, component);
    }

    public void ReplaceInterfaceToGenerate(My.Namespace.InterfaceToGenerate newValue) {
        var component = CreateComponent<InterfaceToGenerateComponent>(TestComponentsLookup.InterfaceToGenerate);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.InterfaceToGenerate, component);
    }

    public void RemoveInterfaceToGenerate() {
        RemoveComponent(TestComponentsLookup.InterfaceToGenerate);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherInterfaceToGenerate;

    public static Entitas.IMatcher<TestEntity> InterfaceToGenerate {
        get {
            if(_matcherInterfaceToGenerate == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.InterfaceToGenerate);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherInterfaceToGenerate = matcher;
            }

            return _matcherInterfaceToGenerate;
        }
    }
}
