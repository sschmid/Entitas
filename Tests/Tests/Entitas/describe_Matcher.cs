using NSpec;
using Entitas;

class describe_Matcher : nspec {
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
            IMatcher m = null;
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

            it["merges matchers to new matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AllOf(new [] { CID.ComponentC });

                var mergedMatcher = Matcher.AllOf(m1, m2, m3);
                mergedMatcher.should_not_be_same(m1);
                mergedMatcher.should_not_be_same(m2);
                mergedMatcher.should_not_be_same(m3);
                mergedMatcher.indices.Length.should_be(3);
                mergedMatcher.indices.should_contain(CID.ComponentA);
                mergedMatcher.indices.should_contain(CID.ComponentB);
                mergedMatcher.indices.should_contain(CID.ComponentC);
            };

            it["can ToString"] = () => m.ToString().should_be("AllOf(1, 2)");
        };

        context["equals"] = () => {
            it["equals equal AllOfMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["equals equal AllOfMatcher independent from the order of indices"] = () => {
                var m1 = allOfAB();
                var m2 = Matcher.AllOf(new [] {
                    CID.ComponentB,
                    CID.ComponentA
                });

                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
            };

            it["equals merged matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AllOf(new [] { CID.ComponentB, CID.ComponentA });

                var mergedMatcher = Matcher.AllOf(m1, m2);
                mergedMatcher.Equals(m3).should_be_true();
            };

            it["doesn't equal different AllOfMatcher"] = () => {
                var m1 = Matcher.AllOf(new [] {
                    CID.ComponentA
                });
                var m2 = allOfAB();

                m1.Equals(m2).should_be_false();
            };

            it["generates same hash for equal AllOfMatcher"] = () => {
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

    IMatcher allOfAB() {
        return Matcher.AllOf(new [] {
            CID.ComponentA,
            CID.ComponentB
        });
    }
}

