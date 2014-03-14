using NSpec;
using Entitas;

class describe_EntityMatcher :nspec {
    void when_creating_matcher() {
        Entity eA = null;
        Entity eC = null;
        Entity eAB = null;
        Entity eABC = null;
        before = () => {
            eA = new Entity();
            eC = new Entity();
            eAB = new Entity();
            eABC = new Entity();
            eA.AddComponent(new ComponentA());
            eC.AddComponent(new ComponentC());
            eAB.AddComponent(new ComponentA());
            eAB.AddComponent(new ComponentB());
            eABC.AddComponent(new ComponentA());
            eABC.AddComponent(new ComponentB());
            eABC.AddComponent(new ComponentC());
        };

        context["allOf"] = () => {
            IEntityMatcher m = null;
            before = () => m = EntityMatcher.AllOf(new [] {
                typeof(ComponentA),
                typeof(ComponentB)
            });

            it["doesn't match"] = () => {
                m.Matches(eA).should_be_false();
            };

            it["matches"] = () => {
                m.Matches(eAB).should_be_true();
                m.Matches(eABC).should_be_true();
            };
        };

        context["anyOf"] = () => {
            IEntityMatcher m = null;
            before = () => m = EntityMatcher.AnyOf(new [] {
                typeof(ComponentA),
                typeof(ComponentB)
            });

            it["doesn't match"] = () => {
                m.Matches(eC).should_be_false();
            };

            it["matches"] = () => {
                m.Matches(eA).should_be_true();
            };
        };


        context["equals"] = () => {
            it["equals equal AllOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });
                var m2 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["doesn't equal different AllOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA)
                });
                var m2 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.Equals(m2).should_be_false();
            };

            it["generates same hash for equal AllOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });
                var m2 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.GetHashCode().should_be(m2.GetHashCode());
            };
                
            it["equals equal AnyOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });
                var m2 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["doesn't equal different AnyOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA)
                });
                var m2 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.Equals(m2).should_be_false();
            };

            it["generates same hash for equal AnyOfEntityMatcher"] = () => {
                var m1 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });
                var m2 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.GetHashCode().should_be(m2.GetHashCode());
            };

            it["generates different hash for different IEntityMatcher with equal sets"] = () => {
                var m1 = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });
                var m2 = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m1.GetHashCode().should_not_be(m2.GetHashCode());
            };

            it["allOf can ToString"] = () => {
                var m = EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m.ToString().should_be("AllOfEntityMatcher(ComponentA, ComponentB)");
            };

            it["anyOf can ToString"] = () => {
                var m = EntityMatcher.AnyOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                });

                m.ToString().should_be("AnyOfEntityMatcher(ComponentA, ComponentB)");
            };
        };
    }
}

