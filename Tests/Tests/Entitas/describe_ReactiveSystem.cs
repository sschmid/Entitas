using Entitas;
using NSpec;

class describe_ReactiveSystem : nspec {

    readonly IMatcher _matcherAB = Matcher.AllOf(CID.ComponentA, CID.ComponentB);

    static void assertEntities(IReactiveSubSystemSpy system, Entity entity, int didExecute = 1) {
        if(entity == null) {
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
                reactiveSystem.Execute();
                assertEntities(subSystem, null);
            };

            it["executes when triggered"] = () => {
                var e = createEntityAB();
                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["executes only once when triggered"] = () => {
                var e = createEntityAB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["retains and releases collected entities"] = () => {
                var e = createEntityAB();
                var retainCount = e.retainCount;
                reactiveSystem.Execute();
                retainCount.should_be(3); // retained by pool, group and entity collector
                e.retainCount.should_be(2); // retained by pool and group
            };

            it["collects changed entities in execute"] = () => {
                var e = createEntityAB();
                subSystem.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                reactiveSystem.Execute();
                reactiveSystem.Execute();
                assertEntities(subSystem, e, 2);
            };

            it["collects created entities in execute"] = () => {
                var e1 = createEntityAB();
                Entity e2 = null;
                subSystem.executeAction = entities => {
                    if(e2 == null) {
                        e2 = createEntityAB();
                    }
                };

                reactiveSystem.Execute();
                assertEntities(subSystem, e1);

                reactiveSystem.Execute();
                assertEntities(subSystem, e2, 2);
            };

            it["doesn't execute when not triggered"] = () => {
                _pool.CreateEntity().AddComponentA();
                reactiveSystem.Execute();
                assertEntities(subSystem, null);
            };

            it["deactivates and will not trigger"] = () => {
                reactiveSystem.Deactivate();
                createEntityAB();
                reactiveSystem.Execute();
                assertEntities(subSystem, null);
            };

            it["activates and will trigger again"] = () => {
                reactiveSystem.Deactivate();
                reactiveSystem.Activate();
                var e = createEntityAB();
                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["clears"] = () => {
                createEntityAB();
                reactiveSystem.Clear();
                reactiveSystem.Execute();
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
                var e = createEntityAB()
                    .RemoveComponentA();

                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["executes only once when triggered"] = () => {
                var e = createEntityAB()
                    .RemoveComponentA();

                reactiveSystem.Execute();
                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["doesn't execute when not triggered"] = () => {
                createEntityAB()
                    .AddComponentC()
                    .RemoveComponentC();

                reactiveSystem.Execute();
                assertEntities(subSystem, null);
            };

            it["retains entities until execute completed"] = () => {
                var e = createEntityAB();
                var didExecute = 0;
                subSystem.executeAction = entities => {
                    didExecute += 1;
                    entities[0].retainCount.should_be(1);
                };

                _pool.DestroyEntity(e);
                reactiveSystem.Execute();
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
                var e = createEntityAB();
                reactiveSystem.Execute();
                assertEntities(subSystem, e);
            };

            it["executes when removed"] = () => {
                var e = createEntityAB();
                reactiveSystem.Execute();
                e.RemoveComponentA();
                reactiveSystem.Execute();
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
                var eA = _pool.CreateEntity().AddComponentA();
                var eB = _pool.CreateEntity().AddComponentB();
                reactiveSystem.Execute();
                assertEntities(multiSubSystem, eA);
                eB.RemoveComponentB();
                reactiveSystem.Execute();
                assertEntities(multiSubSystem, eB, 2);
            };
        };

        context["EntityCollectorSystem"] = () => {

            EntityCollectorSubSystemSpy entityCollectorSubSystem = null;
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
                var entityCollector = new EntityCollector(groups, eventTypes);

                entityCollectorSubSystem = new EntityCollectorSubSystemSpy(entityCollector);
                reactiveSystem = new ReactiveSystem(entityCollectorSubSystem);
            };

            it["executes when a triggered by entityCollector"] = () => {
                var eA1 = pool1.CreateEntity().AddComponentA();
                pool2.CreateEntity().AddComponentA();

                var eB1 = pool1.CreateEntity().AddComponentB();
                var eB2 = pool2.CreateEntity().AddComponentB();

                reactiveSystem.Execute();
                assertEntities(entityCollectorSubSystem, eA1);

                eB1.RemoveComponentB();
                eB2.RemoveComponentB();
                reactiveSystem.Execute();
                assertEntities(entityCollectorSubSystem, eB2, 2);
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
                        reactiveSystem.Execute();
                        assertEntities(ensureSubSystem, eABC);
                    };

                    it["retains included entities until execute completed"] = () => {
                        var retainCount = eABC.retainCount;
                        var didExecute = 0;
                        ensureSubSystem.executeAction = entities => {
                            didExecute += 1;
                            eABC.retainCount.should_be(3);
                        };

                        reactiveSystem.Execute();
                        didExecute.should_be(1);
                        retainCount.should_be(3); // retained by pool, group and entity collector
                        eABC.retainCount.should_be(2); // retained by pool and group
                    };

                    it["doesn't retain not included entities until execute completed"] = () => {
                        var retainCount = eAB.retainCount;
                        var didExecute = 0;
                        ensureSubSystem.executeAction = entity => {
                            didExecute += 1;
                            eAB.retainCount.should_be(2);
                        };

                        reactiveSystem.Execute();
                        didExecute.should_be(1);
                        retainCount.should_be(3); // retained by pool, group and entity collector
                        eAB.retainCount.should_be(2); // retained by pool and group
                        eABC.retainCount.should_be(2); // retained by pool and group
                    };

                    it["doesn't call execute when no entities left after filtering"] = () => {

                        ensureSubSystem = new ReactiveEnsureSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, Matcher.AllOf(
                            CID.ComponentA,
                            CID.ComponentB,
                            CID.ComponentC,
                            CID.ComponentD
                        ));

                        reactiveSystem = new ReactiveSystem(_pool, ensureSubSystem);

                        createEntityAB();
                        createEntityABC();
                        reactiveSystem.Execute();
                        assertEntities(ensureSubSystem, null);
                    };
                };


                context["multi reactive system"] = () => {

                    it["only passes in entities matching required matcher"] = () => {
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
                        reactiveSystem.Execute();
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
                        reactiveSystem.Execute();
                        assertEntities(excludeSubSystem, eAB);
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

                context["multi reactive system"] = () => {
                    
                    it["only passes in entities not matching required matcher"] = () => {
                        var triggers = new [] {
                            Matcher.AllOf(CID.ComponentA).OnEntityAdded(),
                            Matcher.AllOf(CID.ComponentB).OnEntityAdded()
                        };
                        var exclude = Matcher.AllOf(CID.ComponentC);

                        var excludeSubSystem = new MultiReactiveExcludeSubSystemSpy(triggers, exclude);
                        reactiveSystem = new ReactiveSystem(_pool, excludeSubSystem);

                        eAB = createEntityAB();
                        createEntityABC();
                        reactiveSystem.Execute();
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
                    reactiveSystem.Execute();
                    assertEntities(ensureExcludeSystem, eAB);
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
        };

        context["filter entities"] = () => {

            it["filters entities"] = () => {
                var filterEntitiesSystem = new ReactiveFilterEntitiesSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded,
                                                    e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                reactiveSystem = new ReactiveSystem(_pool, filterEntitiesSystem);

                var eAC = _pool.CreateEntity()
                                               .AddComponentA()
                                               .AddComponentC();

                var eAB1 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 50 });
                
                var didExecute = 0;
                filterEntitiesSystem.executeAction = entities => {
                    didExecute += 1;
                    eAB2.retainCount.should_be(3); // retained by pool, group and entity collector
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);

                reactiveSystem.Execute();

                filterEntitiesSystem.entities.Length.should_be(1);
                filterEntitiesSystem.entities[0].should_be_same(eAB2);

                eAB1.retainCount.should_be(2); // retained by pool and group
                eAB2.retainCount.should_be(2);
            };

            it["filters entities when ensure"] = () => {
                var filterEntitiesSystem = new ReactiveEnsureFilterEntitiesSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, _matcherAB,
                                                    e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                reactiveSystem = new ReactiveSystem(_pool, filterEntitiesSystem);

                var eAC = _pool.CreateEntity()
                               .AddComponentA()
                               .AddComponentC();

                var eAB1 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 50 });

                var didExecute = 0;
                filterEntitiesSystem.executeAction = entities => {
                    didExecute += 1;
                    eAB2.retainCount.should_be(3); // retained by pool, group and entity collector
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);

                reactiveSystem.Execute();

                filterEntitiesSystem.entities.Length.should_be(1);
                filterEntitiesSystem.entities[0].should_be_same(eAB2);

                eAB1.retainCount.should_be(2); // retained by pool and group
                eAB2.retainCount.should_be(2);
            };

            it["filters entities when exclude"] = () => {
                var filterEntitiesSystem = new ReactiveExcludeFilterEntitiesSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, Matcher.AllOf(CID.ComponentC),
                                                    e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                reactiveSystem = new ReactiveSystem(_pool, filterEntitiesSystem);

                var eAC = _pool.CreateEntity()
                               .AddComponentA()
                               .AddComponentC();

                var eAB1 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 50 });

                var didExecute = 0;
                filterEntitiesSystem.executeAction = entities => {
                    didExecute += 1;
                    eAB2.retainCount.should_be(3); // retained by pool, group and entity collector
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);

                reactiveSystem.Execute();

                filterEntitiesSystem.entities.Length.should_be(1);
                filterEntitiesSystem.entities[0].should_be_same(eAB2);

                eAB1.retainCount.should_be(2); // retained by pool and group
                eAB2.retainCount.should_be(2);
            };

            it["filters entities when ensure and exclude"] = () => {
                var filterEntitiesSystem = new ReactiveEnsureExcludeFilterEntitiesSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded, _matcherAB, Matcher.AllOf(CID.ComponentC),
                                                    e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                reactiveSystem = new ReactiveSystem(_pool, filterEntitiesSystem);

                var eAC = _pool.CreateEntity()
                               .AddComponentA()
                               .AddComponentC();

                var eAB1 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _pool.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 50 });

                var didExecute = 0;
                filterEntitiesSystem.executeAction = entities => {
                    didExecute += 1;
                    eAB2.retainCount.should_be(3); // retained by pool, group and entity collector
                };

                reactiveSystem.Execute();
                didExecute.should_be(1);

                reactiveSystem.Execute();

                filterEntitiesSystem.entities.Length.should_be(1);
                filterEntitiesSystem.entities[0].should_be_same(eAB2);

                eAB1.retainCount.should_be(2); // retained by pool and group
                eAB2.retainCount.should_be(2);
            };
        };

        context["IClearReactiveSystem"] = () => {

            ClearReactiveSubSystemSpy clearSubSystem = null;

            before = () => {
                clearSubSystem = new ClearReactiveSubSystemSpy(_matcherAB, GroupEventType.OnEntityAdded);
                reactiveSystem = new ReactiveSystem(_pool, clearSubSystem);
            };

            it["clears reactive system after execute when implementing IClearReactiveSystem"] = () => {
                subSystem.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                var e = createEntityAB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();
                assertEntities(clearSubSystem, e);
            };
        };
    }
}
