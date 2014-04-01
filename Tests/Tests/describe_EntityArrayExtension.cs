using NSpec;
using Entitas;

class describe_EntityArrayExtension : nspec {
    void when_filtering() {
        Entity eA = null;
        Entity eAB = null;
        Entity eABC = null;
        Entity[] allEntities = null;
        before = () => {
            eA = new Entity();
            eA.AddComponent(new ComponentA());

            eAB = new Entity();
            eAB.AddComponent(new ComponentA());
            eAB.AddComponent(new ComponentB());

            eABC = new Entity();
            eABC.AddComponent(new ComponentA());
            eABC.AddComponent(new ComponentB());
            eABC.AddComponent(new ComponentC());

            allEntities = new[] { eA, eAB, eABC };
        };

        it["removes entities which have not specific components"] = () => {
            var entities = allEntities.With(new [] { typeof(ComponentC) });
            entities.Count.should_be(1);
            entities.should_contain(eABC);
        };

        it["removes entities which have specific components"] = () => {
            var entities = allEntities.Without(new [] { typeof(ComponentC) });
            entities.Count.should_be(2);
            entities.should_not_contain(eABC);
        };
    }
}

