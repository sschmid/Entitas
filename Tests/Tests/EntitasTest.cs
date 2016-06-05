using NSpec;
using Entitas;

class EntitasTest : nspec {

    protected Pool _pool;
    protected Entity _entity;

    protected Entity createEntity() {
        return _pool.CreateEntity();
    }

    protected Entity createEntityA() {
        return createEntity().AddComponentA();
    }

    protected Matcher createMatcherA() {
        return (Matcher)Matcher.AllOf(CID.ComponentA);
    }

    protected Group getGroupA() {
        return _pool.GetGroup(createMatcherA());
    }

    protected PrimaryEntityIndex<string> createPrimaryIndex() {
        return new PrimaryEntityIndex<string>(getGroupA(), c => ((NameAgeComponent)c).name);
    }

    protected NameAgeComponent createNameAge(string name = "Max", int age = 42) {
        var nameAgeComponent = new NameAgeComponent();
        nameAgeComponent.name = name;
        nameAgeComponent.age = age;
        return nameAgeComponent;
    }

    protected void addNameAge(Entity entity) {
        entity.AddComponent(CID.ComponentA, createNameAge());
    }
}

