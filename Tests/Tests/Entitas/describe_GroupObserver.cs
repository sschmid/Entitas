using NSpec;
using Entitas;

class describe_GroupObserver : nspec {

    Pool _pool;

    void when_created() {

        Group groupA = null;
        GroupObserver observerA = null;

        IMatcher matcherA = Matcher.AllOf(new[] { CID.ComponentA });

        before = () => {
            _pool = new Pool(CID.NumComponents);
            groupA = _pool.GetGroup(matcherA);
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observerA = new GroupObserver(groupA, GroupEventType.OnEntityAdded);
            };

            it["is empty when nothing happend"] = () => {
                observerA.collectedEntities.should_be_empty();
            };

            context["when entity collected"] = () => {
                Entity e = null;
                before = () => {
                    e = createEA();
                };

                it["returns collected entities"] = () => {
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["only collects matching entities"] = () => {
                    createEB();
                
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["collects entities only once"] = () => {
                    e.RemoveComponentA();
                    e.AddComponentA();

                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["clears collected entities on deactivation"] = () => {
                    observerA.Deactivate();
                    observerA.collectedEntities.should_be_empty();
                };

                it["doesn't collect entities when deactivated"] = () => {
                    observerA.Deactivate();
                    createEA();
                    observerA.collectedEntities.should_be_empty();
                };
                
                it["continues collecting when activated"] = () => {
                    observerA.Deactivate();
                    createEA();

                    observerA.Activate();

                    var e2 = createEA();

                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e2);
                };

                it["clears collected entities"] = () => {
                    observerA.ClearCollectedEntities();
                    observerA.collectedEntities.should_be_empty();
                };

                it["can ToString"] = () => {
                    observerA.ToString().should_be("GroupObserver(Group(AllOf(1)))");
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
                    observerA.ClearCollectedEntities();
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
                observerA = new GroupObserver(groupA, GroupEventType.OnEntityRemoved);
            };

            it["returns collected entities"] = () => {
                var e = createEA();
                observerA.collectedEntities.should_be_empty();

                e.RemoveComponentA();
                var entities = observerA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
            before = () => {
                observerA = new GroupObserver(groupA, GroupEventType.OnEntityAddedOrRemoved);
            };

            it["returns collected entities"] = () => {
                var e = createEA();
                var entities = observerA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
                observerA.ClearCollectedEntities();

                e.RemoveComponentA();
                entities = observerA.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing multiple groups"] = () => {

            Group groupB = null;
            before = () => {
                groupB = _pool.GetGroup(Matcher.AllOf(new[] { CID.ComponentB }));
            };

            it["throws when group count != eventType count"] = expect<GroupObserverException>(() => {
                observerA = new GroupObserver(
                    new [] { groupA },
                    new [] {
                        GroupEventType.OnEntityAdded,
                        GroupEventType.OnEntityAdded
                    }
                );
            });

            context["when observing with eventType OnEntityAdded"] = () => {
                before = () => {
                    observerA = new GroupObserver(
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

                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };

                it["can ToString"] = () => {
                    observerA.ToString().should_be("GroupObserver(Group(AllOf(1)), Group(AllOf(2)))");
                };
            };

            context["when observing with eventType OnEntityRemoved"] = () => {
                before = () => {
                    observerA = new GroupObserver(
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
                    observerA.collectedEntities.should_be_empty();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
                before = () => {
                    observerA = new GroupObserver(
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
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                    observerA.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = observerA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with mixed eventTypes"] = () => {
                before = () => {
                    observerA = new GroupObserver(
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
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eA);
                    observerA.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eB);
                };
            };
        };
    }

    Entity createEA() {
        var e = _pool.CreateEntity();
        e.AddComponentA();
        return e;
    }

    Entity createEB() {
        var e = _pool.CreateEntity();
        e.AddComponentB();
        return e;
    }
}

