public partial class TestEntity {

    public My.Namespace.MyNamespaceComponent myNamespace { get { return (My.Namespace.MyNamespaceComponent)GetComponent(TestComponentsLookup.MyNamespace); } }
    public bool hasMyNamespace { get { return HasComponent(TestComponentsLookup.MyNamespace); } }

    public void AddMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.MyNamespaceComponent>(TestComponentsLookup.MyNamespace);
        component.value = newValue;
        AddComponent(TestComponentsLookup.MyNamespace, component);
    }

    public void ReplaceMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.MyNamespaceComponent>(TestComponentsLookup.MyNamespace);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.MyNamespace, component);
    }

    public void RemoveMyNamespace() {
        RemoveComponent(TestComponentsLookup.MyNamespace);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherMyNamespace;

    public static Entitas.IMatcher<TestEntity> MyNamespace {
        get {
            if(_matcherMyNamespace == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.MyNamespace);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherMyNamespace = matcher;
            }

            return _matcherMyNamespace;
        }
    }
}
