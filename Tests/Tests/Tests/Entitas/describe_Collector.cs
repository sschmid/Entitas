using Entitas;
using NSpec;

class describe_Collector : nspec {

    IContext<TestEntity> _context;

    void when_created() {

        IGroup<TestEntity> groupA = null;
        ICollector<TestEntity> collectorA = null;
        IMatcher<TestEntity> matcherA = Matcher<TestEntity>.AllOf(CID.ComponentA);

        before = () => {
            _context = new MyTestContext();
            groupA = _context.GetGroup(matcherA);
        };

        context["when observing with GroupEvent.Added"] = () => {

            before = () => {
                collectorA = new Collector<TestEntity>(groupA, GroupEvent.Added);
            };

            it["is empty when nothing happend"] = () => {
                collectorA.collectedEntities.should_be_empty();
            };

            context["when entity collected"] = () => {

                TestEntity e = null;

                before = () => {
                    e = createEA();
                };

                it["returns collected entities"] = () => {
                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["only collects matching entities"] = () => {
                    createEB();

                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["collects entities only once"] = () => {
                    e.RemoveComponentA();
                    e.AddComponentA();

                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["clears collected entities"] = () => {
                    collectorA.ClearCollectedEntities();
                    collectorA.collectedEntities.should_be_empty();
                };

                it["clears collected entities on deactivation"] = () => {
                    collectorA.Deactivate();
                    collectorA.collectedEntities.should_be_empty();
                };

                it["doesn't collect entities when deactivated"] = () => {
                    collectorA.Deactivate();
                    createEA();
                    collectorA.collectedEntities.should_be_empty();
                };

                it["continues collecting when activated"] = () => {
                    collectorA.Deactivate();
                    createEA();

                    collectorA.Activate();

                    var e2 = createEA();

                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e2);
                };

                it["can ToString"] = () => {
                    collectorA.ToString().should_be("Collector(Group(AllOf(1)))");
                };
            };

            context["reference counting"] = () => {

                TestEntity e = null;

                before = () => {
                    e = createEA();
                };

                it["retains entity even after destroy"] = () => {
                    var didExecute = 0;
                    e.OnEntityReleased += delegate { didExecute += 1; };
                    e.Destroy();
                    e.retainCount.should_be(1);

                    var safeAerc = e.aerc as SafeAERC;
                    if (safeAerc != null) {
                        safeAerc.owners.should_contain(collectorA);
                    }

                    didExecute.should_be(0);
                };

                it["releases entity when clearing collected entities"] = () => {
                    e.Destroy();
                    collectorA.ClearCollectedEntities();
                    e.retainCount.should_be(0);
                };

                it["retains entities only once"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    e.Destroy();
                    e.retainCount.should_be(1);
                };
            };
        };

        context["when observing with GroupEvent.Removed"] = () => {

            before = () => {
                collectorA = new Collector<TestEntity>(groupA, GroupEvent.Removed);
            };

            it["returns collected entities"] = () => {
                var e = createEA();
                collectorA.collectedEntities.should_be_empty();

                e.RemoveComponentA();
                var entities = collectorA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing with GroupEvent.AddedOrRemoved"] = () => {

            before = () => {
                collectorA = new Collector<TestEntity>(groupA, GroupEvent.AddedOrRemoved);
            };

            it["returns collected entities"] = () => {
                var e = createEA();
                var entities = collectorA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
                collectorA.ClearCollectedEntities();

                e.RemoveComponentA();
                entities = collectorA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing multiple groups"] = () => {

            IGroup<TestEntity> groupB = null;

            before = () => {
                groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
            };

            it["throws when group count != groupEvent count"] = expect<CollectorException>(() => {
                collectorA = new Collector<TestEntity>(
                    new [] { groupA },
                    new [] {
                        GroupEvent.Added,
                        GroupEvent.Added
                    }
                );
            });

            context["when observing with GroupEvent.Added"] = () => {

                before = () => {
                    collectorA = new Collector<TestEntity>(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEvent.Added,
                            GroupEvent.Added
                        }
                    );
                };

                it["returns collected entities"] = () => {
                    var eA = createEA();
                    var eB = createEB();

                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };

                it["can ToString"] = () => {
                    collectorA.ToString().should_be("Collector(Group(AllOf(1)), Group(AllOf(2)))");
                };
            };

            context["when observing with GroupEvent.Removed"] = () => {

                before = () => {
                    collectorA = new Collector<TestEntity>(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEvent.Removed,
                            GroupEvent.Removed
                        }
                    );
                };
                it["returns collected entities"] = () => {
                    var eA = createEA();
                    var eB = createEB();
                    collectorA.collectedEntities.should_be_empty();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with GroupEvent.AddedOrRemoved"] = () => {

                before = () => {
                    collectorA = new Collector<TestEntity>(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEvent.AddedOrRemoved,
                            GroupEvent.AddedOrRemoved
                        }
                    );
                };
                it["returns collected entities"] = () => {
                    var eA = createEA();
                    var eB = createEB();
                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                    collectorA.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = collectorA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with mixed groupEvents"] = () => {

                before = () => {
                    collectorA = new Collector<TestEntity>(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEvent.Added,
                            GroupEvent.Removed
                        }
                    );
                };
                it["returns collected entities"] = () => {
                    var eA = createEA();
                    var eB = createEB();
                    var entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eA);
                    collectorA.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = collectorA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eB);
                };
            };
        };
    }

    TestEntity createEA() {
        return _context.CreateEntity().AddComponentA();
    }

    TestEntity createEB() {
        return _context.CreateEntity().AddComponentB();
    }
}
