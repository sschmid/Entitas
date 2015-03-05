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
        };

        context["anyOf"] = () => {
            IMatcher m = null;
            before = () => m = Matcher.AnyOf(new [] {
                CID.ComponentA,
                CID.ComponentA,
                CID.ComponentB
            });

            it["doesn't match"] = () => {
                m.Matches(eC).should_be_false();
            };

            it["matches"] = () => {
                m.Matches(eA).should_be_true();
                m.Matches(eAB).should_be_true();
                m.Matches(eABC).should_be_true();
            };
        };

        context["noneOf"] = () => {
            IMatcher m = null;
            before = () => m = Matcher.NoneOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            });

            it["doesn't match"] = () => {
                m.Matches(eA).should_be_false();
                m.Matches(eAB).should_be_false();
            };

            it["matches"] = () => {
                m.Matches(eC).should_be_true();
                m.Matches(this.CreateEntity()).should_be_true();
            };
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

            it["AllOfMatcher doesn't equal AnyOfMatcher with same indices"] = () => {
                var m1 = Matcher.AllOf(new [] {
                    CID.ComponentA, CID.ComponentB
                });
                var m2 = Matcher.AnyOf(new [] {
                    CID.ComponentA, CID.ComponentB
                });

                m1.Equals(m2).should_be_false();
            };
        };
    }

    void when_compounding_matchers() {

        context["allOf"] = () => {
            AllOfMatcher allAB = null;
            AllOfMatcher allBC = null;
            AnyOfMatcher anyAB = null;
            AnyOfMatcher anyBC = null;
            AllOfCompoundMatcher compound = null;
            before = () => {
                allAB = Matcher.AllOf(new[] {
                    CID.ComponentB,
                    CID.ComponentA
                });
                allBC = Matcher.AllOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
                anyAB = Matcher.AnyOf(new[] {
                    CID.ComponentB,
                    CID.ComponentA
                });
                anyBC = Matcher.AnyOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
            };

            it["has all indices in order"] = () => {
                compound = Matcher.AllOf(allAB, allBC);
                compound.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });
            };

            it["has all indices in order (mixed)"] = () => {
                compound = Matcher.AllOf(allAB, anyBC);
                compound.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });
            };

            it["matches"] = () => {
                compound = Matcher.AllOf(allAB, allBC);
                var e = this.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.AddComponentC();
                compound.Matches(e).should_be_true();
            };

            it["matches (mixed)"] = () => {
                compound = Matcher.AllOf(allAB, anyBC);
                var e = this.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                compound.Matches(e).should_be_true();
            };

            it["doesn't match"] = () => {
                compound = Matcher.AllOf(allAB, allBC);
                var e = this.CreateEntity();
                e.AddComponentB();
                e.AddComponentC();
                compound.Matches(e).should_be_false();
            };

            it["doesn't match (mixed)"] = () => {
                compound = Matcher.AllOf(anyAB, anyBC);
                var e = this.CreateEntity();
                e.AddComponentC();
                compound.Matches(e).should_be_false();
            };
        };

        context["anyOf"] = () => {
            AllOfMatcher allAB = null;
            AllOfMatcher allBC = null;
            AnyOfMatcher anyBC = null;
            AnyOfCompoundMatcher compound = null;
            before = () => {
                allAB = Matcher.AllOf(new[] {
                    CID.ComponentB,
                    CID.ComponentA
                });
                allBC = Matcher.AllOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
                anyBC = Matcher.AnyOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
            };

            it["has all indices in order"] = () => {
                compound = Matcher.AnyOf(allAB, allBC);
                compound.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });
            };

            it["has all indices in order (mixed)"] = () => {
                compound = Matcher.AnyOf(allAB, anyBC);
                compound.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });
            };

            it["matches"] = () => {
                compound = Matcher.AnyOf(allBC, allAB);
                var e = this.CreateEntity();
                e.AddComponentB();
                e.AddComponentC();
                compound.Matches(e).should_be_true();
            };

            it["matches (mixed)"] = () => {
                compound = Matcher.AnyOf(allAB, anyBC);
                var e = this.CreateEntity();
                e.AddComponentC();
                compound.Matches(e).should_be_true();
            };

            it["doesn't match"] = () => {
                compound = Matcher.AnyOf(allAB, allBC);
                var e = this.CreateEntity();
                e.AddComponentA();
                e.AddComponentC();
                compound.Matches(e).should_be_false();
            };
        };

        context["noneOf"] = () => {
            AllOfMatcher allAB = null;
            AllOfMatcher allBC = null;
            AllOfMatcher allAC = null;
            AnyOfMatcher anyBC = null;
            NoneOfCompoundMatcher compound = null;
            before = () => {
                allAB = Matcher.AllOf(new[] {
                    CID.ComponentB,
                    CID.ComponentA
                });
                allBC = Matcher.AllOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
                allAC = Matcher.AllOf(new[] {
                    CID.ComponentC,
                    CID.ComponentA
                });
                anyBC = Matcher.AnyOf(new[] {
                    CID.ComponentC,
                    CID.ComponentB
                });
            };

            it["has all indices in order"] = () => {
                compound = Matcher.NoneOf(allAB, allBC);
                compound.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });
            };

            it["matches"] = () => {
                compound = Matcher.NoneOf(allAB, allAC);
                var e = this.CreateEntity();
                e.AddComponentB();
                e.AddComponentC();
                compound.Matches(e).should_be_true();
            };

            it["matches (mixed)"] = () => {
                compound = Matcher.NoneOf(allAB, anyBC);
                var e = this.CreateEntity();
                e.AddComponentA();
                compound.Matches(e).should_be_true();
            };

            it["doesn't match"] = () => {
                compound = Matcher.NoneOf(allAB, anyBC);
                var e = this.CreateEntity();
                e.AddComponentC();
                compound.Matches(e).should_be_false();
            };
        };

        context["equals"] = () => {
            it["doesn't equal when only indices are same"] = () => {
                var all1 = Matcher.AllOf(0, 1);
                var all2 = Matcher.AllOf(2, 3);
                var c1 = Matcher.AllOf(all1, all2);

                var any1 = Matcher.AnyOf(0, 1);
                var any2 = Matcher.AnyOf(2, 3);
                var c2 = Matcher.AllOf(any1, any2);

                c1.Equals(c2).should_be_false();
            };

            it["doesn't equal when not same type"] = () => {
                var all1 = Matcher.AllOf(0, 1);
                var all2 = Matcher.AllOf(2, 3);
                var c1 = Matcher.AllOf(all1, all2);
                var c2 = Matcher.AnyOf(all1, all2);

                c1.Equals(c2).should_be_false();
            };

            it["equals when equal"] = () => {
                var all1 = Matcher.AllOf(0, 1);
                var all2 = Matcher.AllOf(2, 3);
                var c1 = Matcher.AllOf(all1, all2);

                var all3 = Matcher.AllOf(0, 1);
                var all4 = Matcher.AllOf(2, 3);
                var c2 = Matcher.AllOf(all3, all4);

                c1.Equals(c2).should_be_true();
            };
        };

        context["nested"] = () => {

            it["works like a charme"] = () => {
                var allAB = Matcher.AllOf(CID.ComponentA, CID.ComponentB);
                var allCD = Matcher.AllOf(CID.ComponentC, CID.ComponentD);
                var allEF = Matcher.AllOf(CID.ComponentE, CID.ComponentF);
                var anyEF = Matcher.AnyOf(CID.ComponentE, CID.ComponentF);

                var c1 = Matcher.AllOf(allAB, allCD, anyEF);
                var c2 = Matcher.AllOf(allAB, allCD, allEF);
                var c3 = Matcher.AnyOf(allAB, allCD, allEF);

                var e = this.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.AddComponentC();
                e.AddComponentD();
                e.AddComponentE();

                c1.Matches(e).should_be_true();
                c2.Matches(e).should_be_false();
                c3.Matches(e).should_be_true();

                var nested1 = Matcher.AllOf(c1, c2);
                var nested2 = Matcher.AnyOf(c1, c2);
                nested1.Matches(e).should_be_false();
                nested2.Matches(e).should_be_true();

                nested1.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC,
                    CID.ComponentD,
                    CID.ComponentE,
                    CID.ComponentF
                });
                nested2.indices.should_be(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC,
                    CID.ComponentD,
                    CID.ComponentE,
                    CID.ComponentF
                });

                var nestedAll = Matcher.AllOf(nested1, nested2);
                var nestedAny = Matcher.AnyOf(nested1, nested2);
                nestedAll.Matches(e).should_be_false();
                nestedAny.Matches(e).should_be_true();

                Matcher.NoneOf(nestedAll, nestedAny).Matches(e).should_be_false();
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

