public partial class TestEntity {

    public StructToGenerateComponent structToGenerate { get { return (StructToGenerateComponent)GetComponent(TestComponentsLookup.StructToGenerate); } }
    public bool hasStructToGenerate { get { return HasComponent(TestComponentsLookup.StructToGenerate); } }

    public void AddStructToGenerate(My.Namespace.StructToGenerate newValue) {
        var component = CreateComponent<StructToGenerateComponent>(TestComponentsLookup.StructToGenerate);
        component.value = newValue;
        AddComponent(TestComponentsLookup.StructToGenerate, component);
    }

    public void ReplaceStructToGenerate(My.Namespace.StructToGenerate newValue) {
        var component = CreateComponent<StructToGenerateComponent>(TestComponentsLookup.StructToGenerate);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.StructToGenerate, component);
    }

    public void RemoveStructToGenerate() {
        RemoveComponent(TestComponentsLookup.StructToGenerate);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherStructToGenerate;

    public static Entitas.IMatcher<TestEntity> StructToGenerate {
        get {
            if(_matcherStructToGenerate == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.StructToGenerate);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherStructToGenerate = matcher;
            }

            return _matcherStructToGenerate;
        }
    }
}
