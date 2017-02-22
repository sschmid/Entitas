public partial class TestContext {

    public TestEntity uniqueMyNamespaceFlagEntity { get { return GetGroup(TestMatcher.UniqueMyNamespaceFlag).GetSingleEntity(); } }

    public bool isUniqueMyNamespaceFlag {
        get { return uniqueMyNamespaceFlagEntity != null; }
        set {
            var entity = uniqueMyNamespaceFlagEntity;
            if(value != (entity != null)) {
                if(value) {
                    CreateEntity().isUniqueMyNamespaceFlag = true;
                } else {
                    DestroyEntity(entity);
                }
            }
        }
    }
}

public partial class TestEntity {

    static readonly My.Namespace.UniqueMyNamespaceFlagComponent uniqueMyNamespaceFlagComponent = new My.Namespace.UniqueMyNamespaceFlagComponent();

    public bool isUniqueMyNamespaceFlag {
        get { return HasComponent(TestComponentsLookup.UniqueMyNamespaceFlag); }
        set {
            if(value != isUniqueMyNamespaceFlag) {
                if(value) {
                    AddComponent(TestComponentsLookup.UniqueMyNamespaceFlag, uniqueMyNamespaceFlagComponent);
                } else {
                    RemoveComponent(TestComponentsLookup.UniqueMyNamespaceFlag);
                }
            }
        }
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherUniqueMyNamespaceFlag;

    public static Entitas.IMatcher<TestEntity> UniqueMyNamespaceFlag {
        get {
            if(_matcherUniqueMyNamespaceFlag == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.UniqueMyNamespaceFlag);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherUniqueMyNamespaceFlag = matcher;
            }

            return _matcherUniqueMyNamespaceFlag;
        }
    }
}
