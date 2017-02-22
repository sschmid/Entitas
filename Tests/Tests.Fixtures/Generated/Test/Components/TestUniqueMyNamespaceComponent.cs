public partial class TestContext {

    public TestEntity uniqueMyNamespaceEntity { get { return GetGroup(TestMatcher.UniqueMyNamespace).GetSingleEntity(); } }
    public My.Namespace.UniqueMyNamespaceComponent uniqueMyNamespace { get { return uniqueMyNamespaceEntity.uniqueMyNamespace; } }
    public bool hasUniqueMyNamespace { get { return uniqueMyNamespaceEntity != null; } }

    public TestEntity SetUniqueMyNamespace(string newValue) {
        if(hasUniqueMyNamespace) {
            throw new Entitas.EntitasException("Could not set uniqueMyNamespace!\n" + this + " already has an entity with UniqueMyNamespaceComponent!",
                "You should check if the context already has a uniqueMyNamespaceEntity before setting it or use context.ReplaceUniqueMyNamespace().");
        }
        var entity = CreateEntity();
        entity.AddUniqueMyNamespace(newValue);
        return entity;
    }

    public void ReplaceUniqueMyNamespace(string newValue) {
        var entity = uniqueMyNamespaceEntity;
        if(entity == null) {
            entity = SetUniqueMyNamespace(newValue);
        } else {
            entity.ReplaceUniqueMyNamespace(newValue);
        }
    }

    public void RemoveUniqueMyNamespace() {
        DestroyEntity(uniqueMyNamespaceEntity);
    }
}

public partial class TestEntity {

    public My.Namespace.UniqueMyNamespaceComponent uniqueMyNamespace { get { return (My.Namespace.UniqueMyNamespaceComponent)GetComponent(TestComponentsLookup.UniqueMyNamespace); } }
    public bool hasUniqueMyNamespace { get { return HasComponent(TestComponentsLookup.UniqueMyNamespace); } }

    public void AddUniqueMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.UniqueMyNamespaceComponent>(TestComponentsLookup.UniqueMyNamespace);
        component.value = newValue;
        AddComponent(TestComponentsLookup.UniqueMyNamespace, component);
    }

    public void ReplaceUniqueMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.UniqueMyNamespaceComponent>(TestComponentsLookup.UniqueMyNamespace);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.UniqueMyNamespace, component);
    }

    public void RemoveUniqueMyNamespace() {
        RemoveComponent(TestComponentsLookup.UniqueMyNamespace);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherUniqueMyNamespace;

    public static Entitas.IMatcher<TestEntity> UniqueMyNamespace {
        get {
            if(_matcherUniqueMyNamespace == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.UniqueMyNamespace);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherUniqueMyNamespace = matcher;
            }

            return _matcherUniqueMyNamespace;
        }
    }
}
