using NSpec;
using Entitas;

class describe_EntityArrayExtension : nspec {
    void when_filtering() {
        Entity eA = null;
        Entity eAB = null;
        Entity eABC = null;
        Entity[] allEntities = null;
        before = () => {
            eA = createEntity();
            addComponentA(eA);

            eAB = createEntity();
            addComponentA(eAB);
            addComponentB(eAB);

            eABC = createEntity();
            addComponentA(eABC);
            addComponentB(eABC);
            addComponentC(eABC);

            allEntities = new[] { eA, eAB, eABC };
        };

        it["removes entities which have not specific components"] = () => {
            var entities = allEntities.With(new [] { CP.ComponentC });
            entities.Count.should_be(1);
            entities.should_contain(eABC);
        };

        it["removes entities which have specific components"] = () => {
            var entities = allEntities.Without(new [] { CP.ComponentC });
            entities.Count.should_be(2);
            entities.should_not_contain(eABC);
        };
    }

    Entity createEntity() {
        return new Entity(CP.NumComponents);
    }

    void addComponentA(Entity entity) {
        entity.AddComponent(CP.ComponentA, new ComponentA());
    }

    void addComponentB(Entity entity) {
        entity.AddComponent(CP.ComponentB, new ComponentB());
    }

    void addComponentC(Entity entity) {
        entity.AddComponent(CP.ComponentC, new ComponentC());
    }
}

