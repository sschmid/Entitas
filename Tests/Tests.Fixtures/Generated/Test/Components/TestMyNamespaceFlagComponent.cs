public partial class TestEntity {

    static readonly My.Namespace.MyNamespaceFlagComponent myNamespaceFlagComponent = new My.Namespace.MyNamespaceFlagComponent();

    public bool isMyNamespaceFlag {
        get { return HasComponent(TestComponentsLookup.MyNamespaceFlag); }
        set {
            if(value != isMyNamespaceFlag) {
                if(value) {
                    AddComponent(TestComponentsLookup.MyNamespaceFlag, myNamespaceFlagComponent);
                } else {
                    RemoveComponent(TestComponentsLookup.MyNamespaceFlag);
                }
            }
        }
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherMyNamespaceFlag;

    public static Entitas.IMatcher<TestEntity> MyNamespaceFlag {
        get {
            if(_matcherMyNamespaceFlag == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.MyNamespaceFlag);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherMyNamespaceFlag = matcher;
            }

            return _matcherMyNamespaceFlag;
        }
    }
}
