using NSpec;
using Entitas;

class EntitasTest : nspec {

    protected IContext<TestEntity> _context;
    protected TestEntity _entity;

    protected TestEntity createEntity() {
        return _context.CreateEntity();
    }

    protected IEntity createEntityA() {
        return createEntity().AddComponentA();
    }

    protected Matcher<TestEntity> createMatcherA() {
        return (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA);
    }

    protected IGroup<TestEntity> getGroupA() {
        return _context.GetGroup(createMatcherA());
    }

    protected PrimaryEntityIndex<TestEntity, string> createPrimaryIndex() {
        return new PrimaryEntityIndex<TestEntity, string>(getGroupA(), (e, c) => ((NameAgeComponent)c).name);
    }

    protected NameAgeComponent createNameAge(string name = "Max", int age = 42) {
        var nameAgeComponent = new NameAgeComponent();
        nameAgeComponent.name = name;
        nameAgeComponent.age = age;
        return nameAgeComponent;
    }

    protected void addNameAge(TestEntity entity) {
        entity.AddComponent(CID.ComponentA, createNameAge());
    }
}
