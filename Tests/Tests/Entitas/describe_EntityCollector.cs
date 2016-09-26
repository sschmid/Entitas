using Entitas;
using NSpec;

class describe_EntityCollector : nspec {

    Pool _pool;

    void when_created() {

        Group groupA = null;
        EntityCollector collectorA = null;

        IMatcher matcherA = Matcher.AllOf(CID.ComponentA);

        before = () => {
            _pool = new Pool(CID.TotalComponents);
            groupA = _pool.GetGroup(matcherA);
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            
            before = () => {
                collectorA = new EntityCollector(groupA, GroupEventType.OnEntityAdded);
            };

            it["is empty when nothing happend"] = () => {
                collectorA.collectedEntities.should_be_empty();
            };

            context["when entity collected"] = () => {

                Entity e = null;

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

                it["clears collected entities"] = () => {
                    collectorA.ClearCollectedEntities();
                    collectorA.collectedEntities.should_be_empty();
                };

                it["can ToString"] = () => {
                    collectorA.ToString().should_be("Collector(Group(AllOf(1)))");
                };
            };

            context["reference counting"] = () => {

                Entity e = null;

                before = () => {
                    e = createEA();
                };

                it["retains entity even after destroy"] = () => {
                    var didExecute = 0;
                    e.OnEntityReleased += delegate { didExecute += 1; };
                    _pool.DestroyEntity(e);
                    e.retainCount.should_be(1);
                    didExecute.should_be(0);
                };
                
                it["releases entity when clearing collected entities"] = () => {
                    _pool.DestroyEntity(e);
                    collectorA.ClearCollectedEntities();
                    e.retainCount.should_be(0);
                };

                it["retains entities only once"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    _pool.DestroyEntity(e);
                    e.retainCount.should_be(1);
                };
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {

            before = () => {
                collectorA = new EntityCollector(groupA, GroupEventType.OnEntityRemoved);
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

        context["when observing with eventType OnEntityAddedOrRemoved"] = () => {

            before = () => {
                collectorA = new EntityCollector(groupA, GroupEventType.OnEntityAddedOrRemoved);
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

            Group groupB = null;

            before = () => {
                groupB = _pool.GetGroup(Matcher.AllOf(CID.ComponentB));
            };

            it["throws when group count != eventType count"] = expect<EntityCollectorException>(() => {
                collectorA = new EntityCollector(
                    new [] { groupA },
                    new [] {
                        GroupEventType.OnEntityAdded,
                        GroupEventType.OnEntityAdded
                    }
                );
            });

            context["when observing with eventType OnEntityAdded"] = () => {

                before = () => {
                    collectorA = new EntityCollector(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEventType.OnEntityAdded,
                            GroupEventType.OnEntityAdded
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

            context["when observing with eventType OnEntityRemoved"] = () => {

                before = () => {
                    collectorA = new EntityCollector(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEventType.OnEntityRemoved,
                            GroupEventType.OnEntityRemoved
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

            context["when observing with eventType OnEntityAddedOrRemoved"] = () => {

                before = () => {
                    collectorA = new EntityCollector(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEventType.OnEntityAddedOrRemoved,
                            GroupEventType.OnEntityAddedOrRemoved
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

            context["when observing with mixed eventTypes"] = () => {

                before = () => {
                    collectorA = new EntityCollector(
                        new [] { groupA, groupB },
                        new [] {
                            GroupEventType.OnEntityAdded,
                            GroupEventType.OnEntityRemoved
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

    Entity createEA() {
        return _pool.CreateEntity().AddComponentA();
    }

    Entity createEB() {
        return _pool.CreateEntity().AddComponentB();
    }
}

