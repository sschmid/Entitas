using NSpec;
using Entitas;

class describe_GroupObserver : nspec {

    void when_created() {

        Pool pool = null;
        Group groupA = null;
        GroupObserver observer = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
            groupA = pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observer = new GroupObserver(groupA, GroupEventType.OnEntityAdded);
            };

            it["is empty when nothing happend"] = () => {
                observer.collectedEntities.should_be_empty();
            };

            context["when entity collected"] = () => {
                Entity e = null;
                before = () => {
                    e = pool.CreateEntity();
                    e.AddComponentA();
                };

                it["returns collected entities"] = () => {
                    var entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["only returns matching collected entities"] = () => {
                    var e2 = pool.CreateEntity();
                    e2.AddComponentB();
                
                    var entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["collects entities only once"] = () => {
                    e.RemoveComponentA();
                    e.AddComponentA();

                    var entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e);
                };

                it["clears collected entities on deactivation"] = () => {
                    observer.Deactivate();
                    observer.collectedEntities.should_be_empty();
                };

                it["doesn't collect entities when deactivated"] = () => {
                    observer.Deactivate();
                    var e2 = pool.CreateEntity();
                    e2.AddComponentA();
                    observer.collectedEntities.should_be_empty();
                };
                
                it["continues collecting when activated"] = () => {
                    observer.Deactivate();
                    var e1 = pool.CreateEntity();
                    e1.AddComponentA();

                    observer.Activate();

                    var e2 = pool.CreateEntity();
                    e2.AddComponentA();

                    var entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(e2);
                };

                it["clears collected entities"] = () => {
                    observer.ClearCollectedEntities();
                    observer.collectedEntities.should_be_empty();
                };
            };

            context["reference counting"] = () => {
                it["retains entity even after destroy"] = () => {
                    var e = pool.CreateEntity();
                    e.AddComponentA();
                    e.OnEntityReleased += entity => this.Fail();
                    pool.DestroyEntity(e);
                    e.GetRefCount().should_be(1);
                };
                
                it["releases entity when clearing collected entities"] = () => {
                    var e = pool.CreateEntity();
                    pool.DestroyEntity(e);
                    observer.ClearCollectedEntities();
                    e.GetRefCount().should_be(0);
                };
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {
            before = () => {
                observer = new GroupObserver(groupA, GroupEventType.OnEntityRemoved);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                observer.collectedEntities.should_be_empty();

                e.RemoveComponentA();
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
            before = () => {
                observer = new GroupObserver(groupA, GroupEventType.OnEntityAddedOrRemoved);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
                observer.ClearCollectedEntities();

                e.RemoveComponentA();
                entities = observer.collectedEntities;
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
                observer = new GroupObserver(
                    new [] { groupA },
                    new [] {
                        GroupEventType.OnEntityAdded,
                        GroupEventType.OnEntityAdded
                    }
                );
            });

            context["when observing with eventType OnEntityAdded"] = () => {
                before = () => {
                    observer = new GroupObserver(
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

                    var entities = observer.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with eventType OnEntityRemoved"] = () => {
                before = () => {
                    observer = new GroupObserver(
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
                    observer.collectedEntities.should_be_empty();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    var entities = observer.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
                before = () => {
                    observer = new GroupObserver(
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
                    var entities = observer.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                    observer.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = observer.collectedEntities;
                    entities.Count.should_be(2);
                    entities.should_contain(eA);
                    entities.should_contain(eB);
                };
            };

            context["when observing with mixed eventTypes"] = () => {
                before = () => {
                    observer = new GroupObserver(
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
                    var entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eA);
                    observer.ClearCollectedEntities();

                    eA.RemoveComponentA();
                    eB.RemoveComponentB();
                    entities = observer.collectedEntities;
                    entities.Count.should_be(1);
                    entities.should_contain(eB);
                };
            };
        };
    }
}

