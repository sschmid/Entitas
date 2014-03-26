using NSpec;
using Entitas;

class integration : nspec {
    void when_putting_it_all_together() {
        it["works with EntityMatcher.AllOfExcept"] = () => {
            var repo = new EntityRepository();
            var c = repo.GetCollection(EntityMatcher.AllOfExcept(ES.Types<ComponentA, ComponentB>(), ES.Types<ComponentC>()));
            var e = repo.CreateEntity();
            e.AddComponent<ComponentA>();
            c.GetEntities().should_be_empty();

            e.AddComponent<ComponentB>();
            c.GetEntities().should_contain(e);

            e.ReplaceComponent<ComponentC>();
            c.GetEntities().should_be_empty();
        };
    }
}

