using NSpec;
using Entitas;

class describe_EntityMatcher : nspec {
    void when_creating_matcher() {
        Entity eA = null;
        Entity eC = null;
        Entity eAB = null;
        Entity eABC = null;
        before = () => {
            eA = this.CreateEntity();
            eC = this.CreateEntity();
            eAB = this.CreateEntity();
            eABC = this.CreateEntity();
            eA.AddComponentA();
            eC.AddComponentC();
            eAB.AddComponentA();
            eAB.AddComponentB();
            eABC.AddComponentA();
            eABC.AddComponentB();
            eABC.AddComponentC();
        };

        context["allOf"] = () => {
            IEntityMatcher m = null;
            before = () => m = Matcher.AllOf(new [] {
                CID.ComponentA,
                CID.ComponentA,
                CID.ComponentB
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
                m.indices.should_contain(CID.ComponentA);
                m.indices.should_contain(CID.ComponentB);
            };
        };

        context["equals"] = () => {
            it["equals equal AllOfEntityMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["equals equal AllOfEntityMatcher independent from the order of indices"] = () => {
                var m1 = allOfAB();
                var m2 = Matcher.AllOf(new [] {
                    CID.ComponentB,
                    CID.ComponentA
                });

                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["doesn't equal different AllOfEntityMatcher"] = () => {
                var m1 = Matcher.AllOf(new [] {
                    CID.ComponentA
                });
                var m2 = allOfAB();

                m1.Equals(m2).should_be_false();
            };

            it["generates same hash for equal AllOfEntityMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.GetHashCode().should_be(m2.GetHashCode());
            };

            it["generates same hash independent from the order of indices"] = () => {
                var m1 = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });
                var m2 = Matcher.AllOf(new [] {
                    CID.ComponentB,
                    CID.ComponentA
                });
                m1.GetHashCode().should_be(m2.GetHashCode());
            };
        };
    }

    IEntityMatcher allOfAB() {
        return Matcher.AllOf(new [] {
            CID.ComponentA,
            CID.ComponentB
        });
    }
}

