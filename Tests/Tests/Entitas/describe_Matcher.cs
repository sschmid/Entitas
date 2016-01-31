using NSpec;
using Entitas;

class describe_Matcher : nspec {
    static void assertIndicesContain(int[] indices, params int[] expectedIndices) {
        indices.Length.should_be(expectedIndices.Length);
        for (int i = 0; i < expectedIndices.Length; i++) {
            indices[i].should_be(expectedIndices[i]);
        }
    }

    void when_creating_matcher() {
        Entity eA = null;
        Entity eB = null;
        Entity eC = null;
        Entity eAB = null;
        Entity eABC = null;
        before = () => {
            eA = this.CreateEntity();
            eA.AddComponentA();

            eB = this.CreateEntity();
            eB.AddComponentB();

            eC = this.CreateEntity();
            eC.AddComponentC();

            eAB = this.CreateEntity();
            eAB.AddComponentA();
            eAB.AddComponentB();

            eABC = this.CreateEntity();
            eABC.AddComponentA();
            eABC.AddComponentB();
            eABC.AddComponentC();
        };

        context["allOf"] = () => {
            IAllOfMatcher m = null;
            before = () => m = Matcher.AllOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            });

            it["has all indices"] = () => {
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["has all indices without duplicates"] = () => {
                m = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentB
                });
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["caches indices"] = () => m.indices.should_be_same(m.indices);
            it["doesn't match"] = () => m.Matches(eA).should_be_false();
            it["matches"] = () => {
                m.Matches(eAB).should_be_true();
                m.Matches(eABC).should_be_true();
            };

            it["merges matchers to new matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AllOf(new [] { CID.ComponentC });
                var mergedMatcher = Matcher.AllOf(m1, m2, m3);
                assertIndicesContain(mergedMatcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
                assertIndicesContain(mergedMatcher.allOfIndices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            };

            it["merges matchers to new matcher without duplicates"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentA });
                var m3 = Matcher.AllOf(new [] { CID.ComponentB });
                var mergedMatcher = Matcher.AllOf(m1, m2, m3);
                assertIndicesContain(mergedMatcher.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(mergedMatcher.allOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["throws when merging matcher with more than one index"] = expect<MatcherException>(() => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA, CID.ComponentB });
                Matcher.AllOf(m1);
            });

            it["can ToString"] = () => m.ToString().should_be("AllOf(1, 2)");

            it["uses componentNames when set"] = () => {
                var matcher = (Matcher)m;
                matcher.componentNames = new [] { "one", "two", "three" };
                matcher.ToString().should_be("AllOf(two, three)");
            };

            it["uses componentNames when merged matcher ToString"] = () => {
                var m1 = (Matcher)Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = (Matcher)Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = (Matcher)Matcher.AllOf(new [] { CID.ComponentC });

                m2.componentNames = new [] {"m_0", "m_1", "m_2", "m_3"};

                var mergedMatcher = Matcher.AllOf(m1, m2, m3);
                mergedMatcher.ToString().should_be("AllOf(m_1, m_2, m_3)");
            };
        };

        context["anyOf"] = () => {
            IAnyOfMatcher m = null;
            before = () => m = Matcher.AnyOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            });

            it["has all indices"] = () => {
                m = Matcher.AnyOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.anyOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["has all indices without duplicates"] = () => {
                m = Matcher.AnyOf(new [] {
                    CID.ComponentA,
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentB
                });
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.anyOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["caches indices"] = () => m.indices.should_be_same(m.indices);
            it["doesn't match"] = () => m.Matches(eC).should_be_false();
            it["matches"] = () => {
                m.Matches(eA).should_be_true();
                m.Matches(eB).should_be_true();
                m.Matches(eABC).should_be_true();
            };

            it["merges matchers to new matcher"] = () => {
                var m1 = Matcher.AnyOf(new [] { CID.ComponentA });
                var m2 = Matcher.AnyOf(new [] { CID.ComponentB });
                var m3 = Matcher.AnyOf(new [] { CID.ComponentC });
                var mergedMatcher = Matcher.AnyOf(m1, m2, m3);
                assertIndicesContain(mergedMatcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
                assertIndicesContain(mergedMatcher.anyOfIndices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            };

            it["merges matchers to new matcher without duplicates"] = () => {
                var m1 = Matcher.AnyOf(new [] { CID.ComponentA });
                var m2 = Matcher.AnyOf(new [] { CID.ComponentB });
                var m3 = Matcher.AnyOf(new [] { CID.ComponentB });
                var mergedMatcher = Matcher.AnyOf(m1, m2, m3);
                assertIndicesContain(mergedMatcher.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(mergedMatcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
            };

            it["throws when merging matcher with more than one index"] = expect<MatcherException>(() => {
                var m1 = Matcher.AnyOf(new [] { CID.ComponentA, CID.ComponentB });
                Matcher.AnyOf(m1);
            });

            it["can ToString"] = () => m.ToString().should_be("AnyOf(1, 2)");
        };

        context["allOf.noneOf"] = () => {
            ICompoundMatcher m = null;
            before = () => m = Matcher.AllOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            }).NoneOf(CID.ComponentC, CID.ComponentD);

            it["has all indices"] = () => {
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.noneOfIndices, CID.ComponentC, CID.ComponentD);
            };

            it["has all indices without duplicates"] = () => {
                m = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentA,
                    CID.ComponentB
                }).NoneOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.noneOfIndices, CID.ComponentB, CID.ComponentC);
            };

            it["caches indices"] = () => m.indices.should_be_same(m.indices);
            it["doesn't match"] = () => m.Matches(eABC).should_be_false();
            it["matches"] = () => m.Matches(eAB).should_be_true();

            it["mutates existing matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = m1.NoneOf(new [] { CID.ComponentB });
                m1.should_be_same(m2);
                assertIndicesContain(m1.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m1.allOfIndices, CID.ComponentA);
                assertIndicesContain(m1.noneOfIndices, CID.ComponentB);
            };

            it["mutates existing merged matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AllOf(m1);
                var m4 = m3.NoneOf(m2);
                m3.should_be_same(m4);
                assertIndicesContain(m3.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m3.allOfIndices, CID.ComponentA);
                assertIndicesContain(m3.noneOfIndices, CID.ComponentB);
            };

            it["can ToString"] = () => m.ToString().should_be("AllOf(1, 2).NoneOf(3, 4)");

            it["uses componentNames when componentNames set"] = () => {
                var matcher = (Matcher)m;
                matcher.componentNames = new [] { "one", "two", "three", "four", "five" };
                matcher.ToString().should_be("AllOf(two, three).NoneOf(four, five)");
            };
        };

        context["anyOf.noneOf"] = () => {
            ICompoundMatcher m = null;
            before = () => m = Matcher.AnyOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            }).NoneOf(CID.ComponentC, CID.ComponentD);

            it["has all indices"] = () => {
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
                assertIndicesContain(m.anyOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.noneOfIndices, CID.ComponentC, CID.ComponentD);
            };

            it["has all indices without duplicates"] = () => {
                m = Matcher.AnyOf(new [] {
                    CID.ComponentA,
                    CID.ComponentA,
                    CID.ComponentB
                }).NoneOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
                assertIndicesContain(m.anyOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.noneOfIndices, CID.ComponentB, CID.ComponentC);
            };

            it["caches indices"] = () => m.indices.should_be_same(m.indices);
            it["doesn't match"] = () => m.Matches(eABC).should_be_false();
            it["matches"] = () => m.Matches(eA).should_be_true();
            it["matches"] = () => m.Matches(eB).should_be_true();

            it["mutates existing matcher"] = () => {
                var m1 = Matcher.AnyOf(new [] { CID.ComponentA });
                var m2 = m1.NoneOf(new [] { CID.ComponentB });
                m1.should_be_same(m2);
                assertIndicesContain(m1.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m1.anyOfIndices, CID.ComponentA);
                assertIndicesContain(m1.noneOfIndices, CID.ComponentB);
            };

            it["mutates existing merged matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AnyOf(m1);
                var m4 = m3.NoneOf(m2);
                m3.should_be_same(m4);
                assertIndicesContain(m3.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m3.anyOfIndices, CID.ComponentA);
                assertIndicesContain(m3.noneOfIndices, CID.ComponentB);
            };

            it["can ToString"] = () => m.ToString().should_be("AnyOf(1, 2).NoneOf(3, 4)");
        };

        context["allOf.anyOf"] = () => {
            ICompoundMatcher m = null;
            before = () => m = Matcher.AllOf(new [] {
                CID.ComponentA,
                CID.ComponentB
            }).AnyOf(CID.ComponentC, CID.ComponentD);

            it["has all indices"] = () => {
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.anyOfIndices, CID.ComponentC, CID.ComponentD);
            };

            it["has all indices without duplicates"] = () => {
                m = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentA,
                    CID.ComponentB
                }).AnyOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
                assertIndicesContain(m.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
                assertIndicesContain(m.allOfIndices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m.anyOfIndices, CID.ComponentB, CID.ComponentC);
            };

            it["caches indices"] = () => m.indices.should_be_same(m.indices);
            it["doesn't match"] = () => m.Matches(eAB).should_be_false();
            it["matches"] = () => m.Matches(eABC).should_be_true();

            it["mutates existing matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = m1.AnyOf(new [] { CID.ComponentB });
                m1.should_be_same(m2);
                assertIndicesContain(m1.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m1.allOfIndices, CID.ComponentA);
                assertIndicesContain(m1.anyOfIndices, CID.ComponentB);
            };

            it["mutates existing merged matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = Matcher.AllOf(m1);
                var m4 = m3.AnyOf(m2);
                m3.should_be_same(m4);
                assertIndicesContain(m3.indices, CID.ComponentA, CID.ComponentB);
                assertIndicesContain(m3.allOfIndices, CID.ComponentA);
                assertIndicesContain(m3.anyOfIndices, CID.ComponentB);
            };

            it["can ToString"] = () => m.ToString().should_be("AllOf(1, 2).AnyOf(3, 4)");
        };

        context["indices cache"] = () => {
            it["updates cache when calling AnyOf"] = () => {
                var m = Matcher.AllOf(new [] { CID.ComponentA });
                var cache = m.indices;
                m.AnyOf(new [] { CID.ComponentB });
                m.indices.should_not_be_same(cache);
            };

            it["updates cache when calling NoneOf"] = () => {
                var m = Matcher.AllOf(new [] { CID.ComponentA });
                var cache = m.indices;
                m.NoneOf(new [] { CID.ComponentB });
                m.indices.should_not_be_same(cache);
            };
        };

        context["equals"] = () => {
            it["equals equal AllOfMatcher"] = () => {
                var m1 = allOfAB();
                var m2 = allOfAB();
                m1.should_not_be_same(m2);
                m1.Equals(m2).should_be_true();
                m1.GetHashCode().should_be(m2.GetHashCode());
            };

            it["equals equal AllOfMatcher independent of the order of indices"] = () => {
                var m1 = allOfAB();
                var m2 = allOfBA();

                m1.Equals(m2).should_be_true();
                m1.GetHashCode().should_be(m2.GetHashCode());
            };

            it["equals merged matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });
                var m3 = allOfBA();

                var mergedMatcher = Matcher.AllOf(m1, m2);
                mergedMatcher.Equals(m3).should_be_true();
                mergedMatcher.GetHashCode().should_be(m3.GetHashCode());
            };

            it["doesn't equal different AllOfMatcher"] = () => {
                var m1 = Matcher.AllOf(new [] {
                    CID.ComponentA
                });
                var m2 = allOfAB();

                m1.Equals(m2).should_be_false();
                m1.GetHashCode().should_not_be(m2.GetHashCode());
            };

            it["allOf doesn't equal anyOf with same indices"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AnyOf(new [] { CID.ComponentA });
                m1.Equals(m2).should_be_false();
                m1.GetHashCode().should_not_be(m2.GetHashCode());
            };

            it["doesn't equal differnt type matchers with same indices"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AllOf(new [] { CID.ComponentB });

                var m3 = Matcher.AllOf(m1, m2);
                var m4 = Matcher.AnyOf(m1, m2);
                m3.Equals(m4).should_be_false();
                m3.GetHashCode().should_not_be(m4.GetHashCode());
            };

            it["equals compound matcher"] = () => {
                var m1 = Matcher.AllOf(new [] { CID.ComponentA });
                var m2 = Matcher.AnyOf(new [] { CID.ComponentB });
                var m3 = Matcher.AnyOf(new [] { CID.ComponentC });
                var m4 = Matcher.AnyOf(new [] { CID.ComponentD });

                var mX = Matcher.AllOf(m1, m2).AnyOf(m3, m4);
                var mY = Matcher.AllOf(m1, m2).AnyOf(m3, m4);

                mX.Equals(mY).should_be_true();
                mX.GetHashCode().should_be(mY.GetHashCode());
            };
        };
    }

    static IMatcher allOfAB() {
        return Matcher.AllOf(new [] { CID.ComponentA, CID.ComponentB });
    }

    static IMatcher allOfBA() {
        return Matcher.AllOf(new [] { CID.ComponentB, CID.ComponentA });
    }
}

