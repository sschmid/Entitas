using NSpec;
using Entitas;

class describe_ReactiveSystem : nspec {

    readonly IMatcher _matcherAB = Matcher.AllOf(new [] {
        CID.ComponentA,
        CID.ComponentB
    });

    static void assertEntities(IReactiveSubSystemSpy system, Entity entity, int didExecute = 1) {
        if (entity == null) {
            system.didExecute.should_be(0);
            system.entities.should_be_null();

        } else {
            system.didExecute.should_be(didExecute);
            system.entities.Length.should_be(1);
            system.entities.should_contain(entity);
        }
    }

    Pool _pool;

    Entity createEntityAB() {
        return _pool.CreateEntity()
            .AddComponentA()
            .AddComponentB();
    }

    Entity createEntityAC() {
        return _pool.CreateEntity()
            .AddComponentA()
            .AddComponentC();
    }

    Entity createEntityABC() {
        return _pool.CreateEntity()
            .AddComponentA()
            .AddComponentB()
            .AddComponentC();
    }

    void when_created() {

        ReactiveSystem reactiveSystem = null;
        ReactiveSubSystemSpy subSystem = null;

        before = () => {
            _pool = new Pool(CID.TotalComponents);
        };

        context["OnEntityAdded"] = () => {

            before = () => {
                subSystem = new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded);
                reactiveSystem = new ReactiveSystem(_pool, subSystem);
            };

            it["does not execute its subsystem when no entities were collected"] = () => {

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, null);
            };

            it["executes when triggered"] = () => {

                // given
                var e = createEntityAB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["executes only once when triggered"] = () => {

                // given
                var e = createEntityAB();

                // when
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["retains and releases collected entities"] = () => {

                // given
                var e = createEntityAB();
                var retainCount = e.retainCount;

                // when
                reactiveSystem.Execute();

                // then
                retainCount.should_be(3); // retained by pool, group and group observer
                e.retainCount.should_be(2); // retained by pool and group
            };

            it["collects changed entities in execute"] = () => {

                // given
                var e = createEntityAB();
                subSystem.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                // when
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e, 2);
            };

            it["collects created entities in execute"] = () => {

                // given
                var e1 = createEntityAB();
                Entity e2 = null;
                subSystem.executeAction = entities => {
                    if (e2 == null) {
                        e2 = createEntityAB();
                    }
                };

                reactiveSystem.Execute();
                assertEntities(subSystem, e1);

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e2, 2);
            };

            it["doesn't execute when not triggered"] = () => {

                // given
                _pool.CreateEntity().AddComponentA();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, null);
            };

            it["deactivates and will not trigger"] = () => {

                // given
                reactiveSystem.Deactivate();
                createEntityAB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, null);
            };

            it["activates and will trigger again"] = () => {

                // given
                reactiveSystem.Deactivate();
                reactiveSystem.Activate();
                var e = createEntityAB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["clears"] = () => {

                // given
                createEntityAB();

                // when
                reactiveSystem.Clear();
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, null);
            };

            it["can ToString"] = () => {
                reactiveSystem.ToString().should_be("ReactiveSystem(ReactiveSubSystemSpy)");
            };
        };

        context["OnEntityRemoved"] = () => {

            before = () => {
                subSystem = new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityRemoved);
                reactiveSystem = new ReactiveSystem(_pool, subSystem);
            };

            it["executes when triggered"] = () => {

                // given
                var e = createEntityAB()
                    .RemoveComponentA();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["executes only once when triggered"] = () => {

                // given
                var e = createEntityAB()
                    .RemoveComponentA();

                // when
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["doesn't execute when not triggered"] = () => {

                // given
                createEntityAB()
                    .AddComponentC()
                    .RemoveComponentC();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, null);
            };

            it["retains entities until execute completed"] = () => {

                // given
                var e = createEntityAB();
                var didExecute = 0;
                subSystem.executeAction = entities => {
                    didExecute += 1;
                    entities[0].retainCount.should_be(1);
                };

                // when
                _pool.DestroyEntity(e);
                reactiveSystem.Execute();

                // then
                didExecute.should_be(1);
                e.retainCount.should_be(0);
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {

            before = () => {
                subSystem = new ReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAddedOrRemoved);
                reactiveSystem = new ReactiveSystem(_pool, subSystem);
            };

            it["executes when added"] = () => {

                // given
                var e = createEntityAB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e);
            };

            it["executes when removed"] = () => {

                // given
                var e = createEntityAB();
                reactiveSystem.Execute();

                // when
                e.RemoveComponentA();
                reactiveSystem.Execute();

                // then
                assertEntities(subSystem, e, 2);
            };
        };

        context["MultiReactiveSystem"] = () => {

            MultiReactiveSubSystemSpy multiSubSystem = null;

            before = () => {
                var triggers = new [] {
                    Matcher.AllOf(CID.ComponentA).OnEntityAdded(),
                    Matcher.AllOf(CID.ComponentB).OnEntityRemoved()
                };
                multiSubSystem = new MultiReactiveSubSystemSpy(triggers);
                reactiveSystem = new ReactiveSystem(_pool, multiSubSystem);
            };

            it["executes when any trigger is triggered"] = () => {

                // given
                var eA = _pool.CreateEntity().AddComponentA();
                var eB = _pool.CreateEntity().AddComponentB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(multiSubSystem, eA);

                // when
                eB.RemoveComponentB();
                reactiveSystem.Execute();

                // then
                assertEntities(multiSubSystem, eB, 2);
            };
        };

        context["GroupObserverSystem"] = () => {

            GroupObserverSubSystemSpy groupObserverSubSystem = null;
            Pool pool1 = null;
            Pool pool2 = null;

            before = () => {
                pool1 = new Pool(CID.TotalComponents);
                pool2 = new Pool(CID.TotalComponents);

                var groupA = pool1.GetGroup(Matcher.AllOf(CID.ComponentA));
                var groupB = pool2.GetGroup(Matcher.AllOf(CID.ComponentB));

                var groups = new [] { groupA, groupB };
                var eventTypes = new [] {
                    GroupEventType.OnEntityAdded,
                    GroupEventType.OnEntityRemoved
                };
                var groupObserver = new GroupObserver(groups, eventTypes);

                groupObserverSubSystem = new GroupObserverSubSystemSpy(groupObserver);
                reactiveSystem = new ReactiveSystem(groupObserverSubSystem);
            };

            it["executes when a triggered by groupObserver"] = () => {

                // given
                var eA1 = pool1.CreateEntity().AddComponentA();
                pool2.CreateEntity().AddComponentA();

                var eB1 = pool1.CreateEntity().AddComponentB();
                var eB2 = pool2.CreateEntity().AddComponentB();

                // when
                reactiveSystem.Execute();

                // then
                assertEntities(groupObserverSubSystem, eA1);

                // when
                eB1.RemoveComponentB();
                eB2.RemoveComponentB();
                reactiveSystem.Execute();

                // then
                assertEntities(groupObserverSubSystem, eB2, 2);
            };
        };

        context["ensure / exlude"] = () => {

            Entity eAB = null;
            Entity eAC = null;
            Entity eABC = null;

            context["ensure components"] = () => {

                context["reactive system"] = () => {

                    ReactiveEnsureSubSystemSpy ensureSubSystem = null;

                    before = () => {
                        ensureSubSystem = new ReactiveEnsureSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, Matcher.AllOf(
                            CID.ComponentA,
                            CID.ComponentB,
                            CID.ComponentC
                        ));

                        reactiveSystem = new ReactiveSystem(_pool, ensureSubSystem);

                        eAB = createEntityAB();
                        eABC = createEntityABC();
                    };

                    it["only passes in entities matching required matcher"] = () => {

                        // when
                        reactiveSystem.Execute();

                        // then
                        assertEntities(ensureSubSystem, eABC);
                    };

                    it["retains included entities until execute completed"] = () => {

                        // given
                        var retainCount = eABC.retainCount;
                        var didExecute = 0;
                        ensureSubSystem.executeAction = entities => {
                            didExecute += 1;
                            eABC.retainCount.should_be(3);
                        };

                        // when
                        reactiveSystem.Execute();

                        // then
                        didExecute.should_be(1);
                        retainCount.should_be(3); // retained by pool, group and group observer
                        eABC.retainCount.should_be(2); // retained by pool and group
                    };

                    it["doesn't retain not included entities until execute completed"] = () => {

                        // given
                        var retainCount = eAB.retainCount;
                        var didExecute = 0;
                        ensureSubSystem.executeAction = entity => {
                            didExecute += 1;
                            eAB.retainCount.should_be(2);
                        };

                        // when
                        reactiveSystem.Execute();

                        // then
                        didExecute.should_be(1);
                        retainCount.should_be(3); // retained by pool, group and group observer
                        eAB.retainCount.should_be(2); // retained by pool and group
                        eABC.retainCount.should_be(2); // retained by pool and group
                    };

                    it["doesn't call execute when no entities left after filtering"] = () => {

                        // given
                        ensureSubSystem = new ReactiveEnsureSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, Matcher.AllOf(
                            CID.ComponentA,
                            CID.ComponentB,
                            CID.ComponentC,
                            CID.ComponentD
                        ));

                        reactiveSystem = new ReactiveSystem(_pool, ensureSubSystem);

                        createEntityAB();
                        createEntityABC();

                        // when
                        reactiveSystem.Execute();

                        // then
                        assertEntities(ensureSubSystem, null);
                    };
                };


                context["multi reactive system"] = () => {

                    it["only passes in entities matching required matcher"] = () => {

                        // given
                        var triggers = new [] {
                            Matcher.AllOf(CID.ComponentA).OnEntityAdded(),
                            Matcher.AllOf(CID.ComponentB).OnEntityAdded()
                        };

                        var ensure = Matcher.AllOf(
                                         CID.ComponentA,
                                         CID.ComponentB,
                                         CID.ComponentC
                                     );

                        var ensureSubSystem = new MultiReactiveEnsureSubSystemSpy(triggers, ensure);
                        reactiveSystem = new ReactiveSystem(_pool, ensureSubSystem);

                        createEntityAB();
                        eABC = createEntityABC();

                        // when
                        reactiveSystem.Execute();

                        // then
                        assertEntities(ensureSubSystem, eABC);
                    };
                };
            };

            context["exlude components"] = () => {

                context["reactive system"] = () => {

                    ReactiveExcludeSubSystemSpy excludeSubSystem = null;

                    before = () => {
                        excludeSubSystem = new ReactiveExcludeSubSystemSpy(_matcherAB,
                            GroupEventType.OnEntityAdded,
                            Matcher.AllOf(CID.ComponentC)
                        );

                        reactiveSystem = new ReactiveSystem(_pool, excludeSubSystem);

                        eAB = createEntityAB();
                        eABC = createEntityABC();
                    };

                    it["only passes in entities not matching matcher"] = () => {

                        // when
                        reactiveSystem.Execute();

                        // then
                        assertEntities(excludeSubSystem, eAB);
                    };

                    it["retains included entities until execute completed"] = () => {

                        // given
                        var didExecute = 0;
                        excludeSubSystem.executeAction = entities => {
                            didExecute += 1;
                            eAB.retainCount.should_be(3);
                        };

                        // when
                        reactiveSystem.Execute();

                        // then
                        didExecute.should_be(1);
                    };

                    it["doesn't retain not included entities until execute completed"] = () => {

                        // given
                        var didExecute = 0;
                        excludeSubSystem.executeAction = entities => {
                            didExecute += 1;
                            eABC.retainCount.should_be(2);
                        };

                        // when
                        reactiveSystem.Execute();

                        // then
                        didExecute.should_be(1);
                    };
                };

                context["multi reactive system"] = () => {
                    
                    it["only passes in entities not matching required matcher"] = () => {

                        // given
                        var triggers = new [] {
                            Matcher.AllOf(CID.ComponentA).OnEntityAdded(),
                            Matcher.AllOf(CID.ComponentB).OnEntityAdded()
                        };
                        var exclude = Matcher.AllOf(CID.ComponentC);

                        var excludeSubSystem = new MultiReactiveExcludeSubSystemSpy(triggers, exclude);
                        reactiveSystem = new ReactiveSystem(_pool, excludeSubSystem);

                        eAB = createEntityAB();
                        createEntityABC();

                        // when
                        reactiveSystem.Execute();

                        // then
                        assertEntities(excludeSubSystem, eAB);
                    };
                };
            };

            context["ensure and exlude mix"] = () => {

                ReactiveEnsureExcludeSubSystemSpy ensureExcludeSystem = null;

                before = () => {
                    ensureExcludeSystem = new ReactiveEnsureExcludeSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded,
                        Matcher.AllOf(CID.ComponentA, CID.ComponentB),
                        Matcher.AllOf(CID.ComponentC)
                    );
                    reactiveSystem = new ReactiveSystem(_pool, ensureExcludeSystem);

                    eAB = createEntityAB();
                    eAC = createEntityAC();
                    eABC = createEntityABC();
                };

                it["only passes in correct entities"] = () => {

                    // when
                    reactiveSystem.Execute();

                    // then
                    assertEntities(ensureExcludeSystem, eAB);
                };

                it["retains included entities until execute completed"] = () => {

                    // given
                    var didExecute = 0;
                    ensureExcludeSystem.executeAction = entities => {
                        didExecute += 1;
                        eAB.retainCount.should_be(3);
                    };

                    // when
                    reactiveSystem.Execute();

                    // then
                    didExecute.should_be(1);
                };

                it["doesn't retain not included entities until execute completed"] = () => {

                    // given

                    var didExecute = 0;
                    ensureExcludeSystem.executeAction = entities => {
                        didExecute += 1;
                        eAC.retainCount.should_be(1);
                        eABC.retainCount.should_be(2);
                    };

                    // when
                    reactiveSystem.Execute();

                    // then
                    didExecute.should_be(1);
                };
            };
        };

        context["IClearReactiveSystem"] = () => {

            ClearReactiveSubSystemSpy clearSubSystem = null;

            before = () => {
                clearSubSystem = new ClearReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded);
                reactiveSystem = new ReactiveSystem(_pool, clearSubSystem);
            };

            it["clears reactive system after execute when implementing IClearReactiveSystem"] = () => {

                // given
                subSystem.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                var e = createEntityAB();

                // when
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                // then
                assertEntities(clearSubSystem, e);
            };
        };
    }
}

