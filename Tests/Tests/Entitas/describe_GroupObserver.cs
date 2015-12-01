using NSpec;
using Entitas;

class describe_GroupObserver : nspec {

    void when_created() {

        Pool pool = null;
        Group groupA = null;
        GroupObserver observerA = null;

        IMatcher mactherA = Matcher.AllOf(new[] { CID.ComponentA });

        before = () => {
            pool = new Pool(CID.NumComponents);
            groupA = pool.GetGroup(mactherA);
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
                    e = pool.CreateEntity();
                    e.AddComponentA();
                };

                it["returns collected entities"] = () => {
                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["only collects matching entities"] = () => {
                    var e2 = pool.CreateEntity();
                    e2.AddComponentB();
                
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
                    var e2 = pool.CreateEntity();
                    e2.AddComponentA();
                    observerA.collectedEntities.should_be_empty();
                };
                
                it["continues collecting when activated"] = () => {
                    observerA.Deactivate();
                    var e1 = pool.CreateEntity();
                    e1.AddComponentA();

                    observerA.Activate();

                    var e2 = pool.CreateEntity();
                    e2.AddComponentA();

                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e2);
                };

                it["clears collected entities"] = () => {
                    observerA.ClearCollectedEntities();
                    observerA.collectedEntities.should_be_empty();
                };
            };

            context["reference counting"] = () => {
                it["retains entity even after destroy"] = () => {
                    var e = pool.CreateEntity();
                    e.AddComponentA();
                    e.OnEntityReleased += entity => this.Fail();
                    pool.DestroyEntity(e);
                    e.retainCount.should_be(1);
                };
                
                it["releases entity when clearing collected entities"] = () => {
                    var e = pool.CreateEntity();
                    e.AddComponentA();
                    pool.DestroyEntity(e);
                    observerA.ClearCollectedEntities();
                    e.retainCount.should_be(0);
                };

                it["retains entities only once"] = () => {
                    var e = pool.CreateEntity();
                    e.AddComponentA();
                    e.ReplaceComponentA(new ComponentA());
                    pool.DestroyEntity(e);
                    e.retainCount.should_be(1);
                };
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {
            before = () => {
                observerA = new GroupObserver(groupA, GroupEventType.OnEntityRemoved);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
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
                var e = pool.CreateEntity();
                e.AddComponentA();
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
                groupB = pool.GetGroup(Matcher.AllOf(new[] { CID.ComponentB }));
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
                    var eA = pool.CreateEntity();
                    eA.AddComponentA();
                    var eB = pool.CreateEntity();
                    eB.AddComponentB();

                    var entities = observerA.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
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
                    var eA = pool.CreateEntity();
                    eA.AddComponentA();
                    var eB = pool.CreateEntity();
                    eB.AddComponentB();
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
                    var eA = pool.CreateEntity();
                    eA.AddComponentA();
                    var eB = pool.CreateEntity();
                    eB.AddComponentB();
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
                    var eA = pool.CreateEntity();
                    eA.AddComponentA();
                    var eB = pool.CreateEntity();
                    eB.AddComponentB();
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
}

