using NSpec;
using Entitas;

class describe_EntityMatcher : nspec {
    void when_creating_matcher() {
        Entity eA = null;
        Entity eC = null;
        Entity eAB = null;
        Entity eABC = null;
        before = () => {
            eA = createEntity();
            eC = createEntity();
            eAB = createEntity();
            eABC = createEntity();
            addComponentA(eA);
            addComponentC(eC);
            addComponentA(eAB);
            addComponentB(eAB);
            addComponentA(eABC);
            addComponentB(eABC);
            addComponentC(eABC);
        };

        context["allOf"] = () => {
            IEntityMatcher m = null;
            before = () => m = EntityMatcher.AllOf(new [] {
                CP.ComponentA,
                CP.ComponentA,
                CP.ComponentB
            });

            it["doesn't match"] = () => {
                m.Matches(eA).should_be_false();
            };

            it["matches"] = () => {
                m.Matches(eAB).should_be_true();
                m.Matches(eABC).should_be_true();
            };

            it["gets triggering types without duplicates"] = () => {
                m.indices.Length.should_be(2);
                m.indices.should_contain(CP.ComponentA);
                m.indices.should_contain(CP.ComponentB);
            };
        };

        context["equals"] = () => {
            it["equals equal AllOfEntityMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["equals equal AllOfEntityMatcher no matter what order"] = () => {
                var m1 = allOfAB();
                var m2 = EntityMatcher.AllOf(new [] {
                    CP.ComponentB,
                    CP.ComponentA
                });
                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["doesn't equal different AllOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AllOf(new [] {
                    CP.ComponentA
                });
                var m2 = allOfAB();

                m1.Equals(m2).should_be_false();
            };

            it["generates same hash for equal AllOfEntityMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.GetHashCode().should_be(m2.GetHashCode());
            };
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

    IEntityMatcher allOfAB() {
        return EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        });
    }
}

