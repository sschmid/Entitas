public partial class TestContext {

    public TestEntity uniqueStandardEntity { get { return GetGroup(TestMatcher.UniqueStandard).GetSingleEntity(); } }
    public UniqueStandardComponent uniqueStandard { get { return uniqueStandardEntity.uniqueStandard; } }
    public bool hasUniqueStandard { get { return uniqueStandardEntity != null; } }

    public TestEntity SetUniqueStandard(string newValue) {
        if(hasUniqueStandard) {
            throw new Entitas.EntitasException("Could not set uniqueStandard!\n" + this + " already has an entity with UniqueStandardComponent!",
                "You should check if the context already has a uniqueStandardEntity before setting it or use context.ReplaceUniqueStandard().");
        }
        var entity = CreateEntity();
        entity.AddUniqueStandard(newValue);
        return entity;
    }

    public void ReplaceUniqueStandard(string newValue) {
        var entity = uniqueStandardEntity;
        if(entity == null) {
            entity = SetUniqueStandard(newValue);
        } else {
            entity.ReplaceUniqueStandard(newValue);
        }
    }

    public void RemoveUniqueStandard() {
        DestroyEntity(uniqueStandardEntity);
    }
}

public partial class TestEntity {

    public UniqueStandardComponent uniqueStandard { get { return (UniqueStandardComponent)GetComponent(TestComponentsLookup.UniqueStandard); } }
    public bool hasUniqueStandard { get { return HasComponent(TestComponentsLookup.UniqueStandard); } }

    public void AddUniqueStandard(string newValue) {
        var component = CreateComponent<UniqueStandardComponent>(TestComponentsLookup.UniqueStandard);
        component.value = newValue;
        AddComponent(TestComponentsLookup.UniqueStandard, component);
    }

    public void ReplaceUniqueStandard(string newValue) {
        var component = CreateComponent<UniqueStandardComponent>(TestComponentsLookup.UniqueStandard);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.UniqueStandard, component);
    }

    public void RemoveUniqueStandard() {
        RemoveComponent(TestComponentsLookup.UniqueStandard);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherUniqueStandard;

    public static Entitas.IMatcher<TestEntity> UniqueStandard {
        get {
            if(_matcherUniqueStandard == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.UniqueStandard);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherUniqueStandard = matcher;
            }

            return _matcherUniqueStandard;
        }
    }
}
