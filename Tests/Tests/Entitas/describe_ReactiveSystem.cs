using NSpec;
using Entitas;

class describe_ReactiveSystem : nspec {

    readonly IMatcher _matcherAB = Matcher.AllOf(new [] {
        CID.ComponentA,
        CID.ComponentB
    });

    void when_created() {

        Pool pool = null;
        ReactiveSystem reactiveSystem = null;
        ReactiveSubSystemSpy subSystem = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        context["OnEntityAdded"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityAdded();
                reactiveSystem = new ReactiveSystem(pool, subSystem);
            };

            it["does not execute its subsystem when no entities were collected"] = () => {
                reactiveSystem.Execute();
                subSystem.didExecute.should_be(0);
            };

            it["executes when trigger matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["executes only once when trigger matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["collects changed entities in execute"] = () => {
                subSystem.replaceComponentAOnExecute = true;
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(2);
            };

            it["doesn't execute when trigger doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                reactiveSystem.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
            };

            it["deactivates and will not be triggered"] = () => {
                reactiveSystem.Deactivate();
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
            };

            it["activates and will be triggered again"] = () => {
                reactiveSystem.Deactivate();
                reactiveSystem.Activate();
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["clears"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Clear();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(0);
            };

            it["can ToString"] = () => {
                reactiveSystem.ToString().should_be("ReactiveSystem(ReactiveSubSystemSpy)");
            };
        };

        context["OnEntityRemoved"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityRemoved();
                reactiveSystem = new ReactiveSystem(pool, subSystem);
            };

            it["executes when trigger matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["executes only once when trigger matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["doesn't execute when trigger doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.AddComponentC();
                e.RemoveComponentC();
                reactiveSystem.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
            };

            it["retains entities until execute completed"] = () => {
                var didExecute = 0;
                Entity providedEntity = null;
                subSystem.executeAction = entities => {
                    didExecute += 1;
                    providedEntity = entities[0];
                    providedEntity.retainCount.should_be(1);
                };

                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                pool.DestroyEntity(e);
                reactiveSystem.Execute();
                didExecute.should_be(1);
                providedEntity.retainCount.should_be(0);
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {
            it["executes when added"] = () => {
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                reactiveSystem = new ReactiveSystem(pool, subSystem);
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["executes when removed"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                reactiveSystem = new ReactiveSystem(pool, subSystem);
                e.RemoveComponentA();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["collects matching entities created or modified in the subsystem"] = () => {
                var sys = new EntityEmittingSubSystem(pool);
                reactiveSystem = new ReactiveSystem(pool, sys);
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();
                sys.entities.Length.should_be(1);
                sys.didExecute.should_be(1);
                reactiveSystem.Execute();
                sys.entities.Length.should_be(1);
                sys.didExecute.should_be(2);
            };
        };

        context["MultiReactiveSystem"] = () => {

            MultiReactiveSubSystemSpy multiSubSystem = null;

            before = () => {
                var triggers = new [] {
                    Matcher.AllOf(new [] { CID.ComponentA }).OnEntityAdded(),
                    Matcher.AllOf(new [] { CID.ComponentB }).OnEntityRemoved()
                };
                multiSubSystem = new MultiReactiveSubSystemSpy(triggers);
                reactiveSystem = new ReactiveSystem(pool, multiSubSystem);
            };

            it["executes when a triggering matcher matches"] = () => {
                var eA = pool.CreateEntity();
                eA.AddComponentA();
                var eB = pool.CreateEntity();
                eB.AddComponentB();
                reactiveSystem.Execute();

                multiSubSystem.didExecute.should_be(1);
                multiSubSystem.entities.Length.should_be(1);
                multiSubSystem.entities.should_contain(eA);

                eB.RemoveComponentB();
                reactiveSystem.Execute();

                multiSubSystem.didExecute.should_be(2);
                multiSubSystem.entities.Length.should_be(1);
                multiSubSystem.entities.should_contain(eB);
            };
        };

        context["GroupObserverSystem"] = () => {

            GroupObserverSubSystemSpy groupObserverSubSystem = null;
            Pool pool1 = null;
            Pool pool2 = null;

            before = () => {
                var triggers = new [] {
                    Matcher.AllOf(new [] { CID.ComponentA }).OnEntityAdded(),
                    Matcher.AllOf(new [] { CID.ComponentB }).OnEntityRemoved()
                };

                pool1 = new Pool(CID.NumComponents);
                pool2 = new Pool(CID.NumComponents);

                var groupA = pool1.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                var groupB = pool2.GetGroup(Matcher.AllOf(new [] { CID.ComponentB }));
                
                var groups = new [] { groupA, groupB };
                var eventTypes = new [] { GroupEventType.OnEntityAdded, GroupEventType.OnEntityRemoved };
                var groupObserver = new GroupObserver(groups, eventTypes);

                groupObserverSubSystem = new GroupObserverSubSystemSpy(groupObserver);
                reactiveSystem = new ReactiveSystem(groupObserverSubSystem);
            };

            it["executes when a triggered by groupObserver"] = () => {
                var eA1 = pool1.CreateEntity();
                eA1.AddComponentA();
                var eA2 = pool2.CreateEntity();
                eA2.AddComponentA();
                
                var eB1 = pool1.CreateEntity();
                eB1.AddComponentB();
                var eB2 = pool2.CreateEntity();
                eB2.AddComponentB();

                reactiveSystem.Execute();

                groupObserverSubSystem.didExecute.should_be(1);
                groupObserverSubSystem.entities.Length.should_be(1);
                groupObserverSubSystem.entities.should_contain(eA1);

                eB1.RemoveComponentB();
                eB2.RemoveComponentB();

                reactiveSystem.Execute();

                groupObserverSubSystem.didExecute.should_be(2);
                groupObserverSubSystem.entities.Length.should_be(1);
                groupObserverSubSystem.entities.should_contain(eB2);
            };
        };

        context["ensure components matcher"] = () => {

            context["single reactive system"] = () => {
                ReactiveEnsureSubSystemSpy ensureSubSystem = null;
                Entity eAB = null;
                Entity eABC = null;
                before = () => {
                    ensureSubSystem = new ReactiveEnsureSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, Matcher.AllOf(new[] {
                        CID.ComponentA,
                        CID.ComponentB,
                        CID.ComponentC
                    }));
                    reactiveSystem = new ReactiveSystem(pool, ensureSubSystem);

                    eAB = pool.CreateEntity();
                    eAB.AddComponentA();
                    eAB.AddComponentB();
                    eABC = pool.CreateEntity();
                    eABC.AddComponentA();
                    eABC.AddComponentB();
                    eABC.AddComponentC();
                };

                it["only passes in entities matching required matcher"] = () => {
                    reactiveSystem.Execute();
                    ensureSubSystem.didExecute.should_be(1);
                    ensureSubSystem.entities.Length.should_be(1);
                    ensureSubSystem.entities.should_contain(eABC);
                };

                it["retains included entities until execute completed"] = () => {
                    eABC.retainCount.should_be(3); // retained by pool, group and group observer
                    var didExecute = 0;
                    ensureSubSystem.executeAction = entities => {
                        didExecute += 1;
                        eABC.retainCount.should_be(3);
                    };
                    reactiveSystem.Execute();
                    didExecute.should_be(1);
                    eABC.retainCount.should_be(2); // retained by pool and group
                };

                it["doesn't retain not included entities until execute completed"] = () => {
                    eAB.retainCount.should_be(3); // retained by pool, group and group observer
                    var didExecute = 0;
                    ensureSubSystem.executeAction = entity => {
                        didExecute += 1;
                        eAB.retainCount.should_be(2);
                    };
                    reactiveSystem.Execute();
                    didExecute.should_be(1);
                    eABC.retainCount.should_be(2); // retained by pool and group
                    eAB.retainCount.should_be(2); // retained by pool and group
                };
            };

            it["only passes in entities matching required matcher (multi reactive)"] = () => {
                var triggers = new [] {
                    Matcher.AllOf(new [] { CID.ComponentA }).OnEntityAdded(),
                    Matcher.AllOf(new [] { CID.ComponentB }).OnEntityRemoved()
                };
                var ensure = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });

                var ensureSubSystem = new MultiReactiveEnsureSubSystemSpy(triggers, ensure);
                reactiveSystem = new ReactiveSystem(pool, ensureSubSystem);

                var eAB = pool.CreateEntity();
                eAB.AddComponentA();
                eAB.AddComponentB();
                var eABC = pool.CreateEntity();
                eABC.AddComponentA();
                eABC.AddComponentB();
                eABC.AddComponentC();
                reactiveSystem.Execute();

                ensureSubSystem.didExecute.should_be(1);
                ensureSubSystem.entities.Length.should_be(1);
                ensureSubSystem.entities.should_contain(eABC);
            };

            it["doesn't call execute when no entities left after filtering"] = () => {
                var ensureSubSystem = new ReactiveEnsureSubSystemSpy(
                                          _matcherAB,
                                          GroupEventType.OnEntityAdded,
                                          Matcher.AllOf(new [] {
                        CID.ComponentA,
                        CID.ComponentB,
                        CID.ComponentC,
                        CID.ComponentD
                    })
                                      );
                reactiveSystem = new ReactiveSystem(pool, ensureSubSystem);

                var eAB = pool.CreateEntity();
                eAB.AddComponentA();
                eAB.AddComponentB();
                var eABC = pool.CreateEntity();
                eABC.AddComponentA();
                eABC.AddComponentB();
                eABC.AddComponentC();
                reactiveSystem.Execute();

                ensureSubSystem.didExecute.should_be(0);
            };
        };

        context["exlude components"] = () => {

            context["single reactive system"] = () => {

                ReactiveExcludeSubSystemSpy excludeSubSystem = null;
                Entity eAB = null;
                Entity eABC = null;
                before = () => {
                    excludeSubSystem = new ReactiveExcludeSubSystemSpy(_matcherAB,
                        GroupEventType.OnEntityAdded,
                        Matcher.AllOf(new[] { CID.ComponentC })
                    );

                    reactiveSystem = new ReactiveSystem(pool, excludeSubSystem);
                    eAB = pool.CreateEntity();
                    eAB.AddComponentA();
                    eAB.AddComponentB();
                    eABC = pool.CreateEntity();
                    eABC.AddComponentA();
                    eABC.AddComponentB();
                    eABC.AddComponentC();
                };

                it["only passes in entities not matching matcher"] = () => {
                    reactiveSystem.Execute();
                    excludeSubSystem.didExecute.should_be(1);
                    excludeSubSystem.entities.Length.should_be(1);
                    excludeSubSystem.entities.should_contain(eAB);
                };

                it["retains included entities until execute completed"] = () => {
                    var didExecute = 0;
                    excludeSubSystem.executeAction = entities => {
                        didExecute += 1;
                        eAB.retainCount.should_be(3);
                    };

                    reactiveSystem.Execute();
                    didExecute.should_be(1);
                };

                it["doesn't retain not included entities until execute completed"] = () => {
                    var didExecute = 0;
                    excludeSubSystem.executeAction = entities => {
                        didExecute += 1;
                        eABC.retainCount.should_be(2);
                    };

                    reactiveSystem.Execute();
                    didExecute.should_be(1);
                };
            };

            it["only passes in entities not matching required matcher (multi reactive)"] = () => {
                var triggers = new [] {
                    Matcher.AllOf(new [] { CID.ComponentA }).OnEntityAdded(),
                    Matcher.AllOf(new [] { CID.ComponentB }).OnEntityRemoved()
                };
                var exclude = Matcher.AllOf(new [] {
                    CID.ComponentC
                });

                var excludeSubSystem = new MultiReactiveExcludeSubSystemSpy(triggers, exclude);
                reactiveSystem = new ReactiveSystem(pool, excludeSubSystem);

                var eAB = pool.CreateEntity();
                eAB.AddComponentA();
                eAB.AddComponentB();
                var eABC = pool.CreateEntity();
                eABC.AddComponentA();
                eABC.AddComponentB();
                eABC.AddComponentC();
                reactiveSystem.Execute();

                excludeSubSystem.didExecute.should_be(1);
                excludeSubSystem.entities.Length.should_be(1);
                excludeSubSystem.entities.should_contain(eAB);
            };
        };

        context["ensure / exlude components mix"] = () => {

            ReactiveEnsureExcludeSubSystemSpy ensureExcludeSystem = null;
            Entity eAB = null;
            Entity eAC = null;
            Entity eABC = null;
            before = () => {
                ensureExcludeSystem = new ReactiveEnsureExcludeSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded,
                    Matcher.AllOf(new[] {
                        CID.ComponentA,
                        CID.ComponentB
                    }), Matcher.AllOf(new[] {
                    CID.ComponentC
                }));
                reactiveSystem = new ReactiveSystem(pool, ensureExcludeSystem);

                eAB = pool.CreateEntity();
                eAB.AddComponentA();
                eAB.AddComponentB();
                eAC = pool.CreateEntity();
                eAC.AddComponentA();
                eAC.AddComponentC();
                eABC = pool.CreateEntity();
                eABC.AddComponentA();
                eABC.AddComponentB();
                eABC.AddComponentC();
            };

            it["only passes in entities"] = () => {
                reactiveSystem.Execute();
                ensureExcludeSystem.didExecute.should_be(1);
                ensureExcludeSystem.entities.Length.should_be(1);
                ensureExcludeSystem.entities.should_contain(eAB);
            };

            it["retains included entities until execute completed"] = () => {
                var didExecute = 0;
                ensureExcludeSystem.executeAction = entities => {
                    didExecute += 1;
                    eAB.retainCount.should_be(3);
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);
            };

            it["doesn't retain not included entities until execute completed"] = () => {
                var didExecute = 0;
                ensureExcludeSystem.executeAction = entities => {
                    didExecute += 1;
                    eAC.retainCount.should_be(1);
                    eABC.retainCount.should_be(2);
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);
            };
        };

        context["IClearReactiveSystem"] = () => {
            ClearReactiveSubSystemSpy clearSubSystem = null;
            before = () => {
                clearSubSystem = getClearSubSystemSypWithOnEntityAdded();
                reactiveSystem = new ReactiveSystem(pool, clearSubSystem);
            };

            it["clears reactive system after execute when implementing IClearReactiveSystem"] = () => {
                clearSubSystem.replaceComponentAOnExecute = true;
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();
                clearSubSystem.didExecute.should_be(1);
            };
        };
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAdded() {
        return new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityRemoved() {
        return new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityRemoved);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAddedOrRemoved() {
        return new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAddedOrRemoved);
    }

    ClearReactiveSubSystemSpy getClearSubSystemSypWithOnEntityAdded() {
        return new ClearReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded);
    }
}

