public partial class TestEntity {

    public ClassToGenerateComponent classToGenerate { get { return (ClassToGenerateComponent)GetComponent(TestComponentsLookup.ClassToGenerate); } }
    public bool hasClassToGenerate { get { return HasComponent(TestComponentsLookup.ClassToGenerate); } }

    public void AddClassToGenerate(My.Namespace.ClassToGenerate newValue) {
        var component = CreateComponent<ClassToGenerateComponent>(TestComponentsLookup.ClassToGenerate);
        component.value = newValue;
        AddComponent(TestComponentsLookup.ClassToGenerate, component);
    }

    public void ReplaceClassToGenerate(My.Namespace.ClassToGenerate newValue) {
        var component = CreateComponent<ClassToGenerateComponent>(TestComponentsLookup.ClassToGenerate);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.ClassToGenerate, component);
    }

    public void RemoveClassToGenerate() {
        RemoveComponent(TestComponentsLookup.ClassToGenerate);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherClassToGenerate;

    public static Entitas.IMatcher<TestEntity> ClassToGenerate {
        get {
            if(_matcherClassToGenerate == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.ClassToGenerate);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherClassToGenerate = matcher;
            }

            return _matcherClassToGenerate;
        }
    }
}
