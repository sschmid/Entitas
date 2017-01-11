using NSpec;
using Entitas;

class EntitasTest : nspec {

    protected Context _context;
    protected IEntity _entity;

    protected IEntity createEntity() {
        return _context.CreateEntity();
    }

    protected IEntity createEntityA() {
        return createEntity().AddComponentA();
    }

    protected Matcher createMatcherA() {
        return (Matcher)Matcher.AllOf(CID.ComponentA);
    }

    protected Group getGroupA() {
        return _context.GetGroup(createMatcherA());
    }

    protected PrimaryEntityIndex<string> createPrimaryIndex() {
        return new PrimaryEntityIndex<string>(getGroupA(), (e, c) => ((NameAgeComponent)c).name);
    }

    protected NameAgeComponent createNameAge(string name = "Max", int age = 42) {
        var nameAgeComponent = new NameAgeComponent();
        nameAgeComponent.name = name;
        nameAgeComponent.age = age;
        return nameAgeComponent;
    }

    protected void addNameAge(IEntity entity) {
        entity.AddComponent(CID.ComponentA, createNameAge());
    }
}
