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

    Pools _pools;

    Entity createEntityAB() {
        return _pools.test.CreateEntity()
            .AddComponentA()
            .AddComponentB();
    }

    Entity createEntityAC() {
        return _pools.test.CreateEntity()
            .AddComponentA()
            .AddComponentC();
    }

    Entity createEntityABC() {
        return _pools.test.CreateEntity()
            .AddComponentA()
            .AddComponentB()
            .AddComponentC();
    }

    void when_created() {

        ReactiveSystem reactiveSystem = null;
        ReactiveSubSystemSpy subSystem = null;

        before = () => {
            _pools = new Pools { test = new Pool(CID.TotalComponents) };
        };

        context["OnEntityAdded"] = () => {

            before = () => {
                subSystem = new ReactiveSubSystemSpy(_pools.test.CreateEntityCollector(_matcherAB));
                reactiveSystem = new ReactiveSystem(subSystem, _pools);
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
                _pools.test.CreateEntity().AddComponentA();
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
                subSystem = new ReactiveSubSystemSpy(_pools.test.CreateEntityCollector(_matcherAB, GroupEventType.OnEntityRemoved));
                reactiveSystem = new ReactiveSystem(subSystem, _pools);
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

                _pools.test.DestroyEntity(e);
                reactiveSystem.Execute();
                didExecute.should_be(1);
                e.retainCount.should_be(0);
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {

            before = () => {
                subSystem = new ReactiveSubSystemSpy(_pools.test.CreateEntityCollector(_matcherAB, GroupEventType.OnEntityAddedOrRemoved));
                reactiveSystem = new ReactiveSystem(subSystem, _pools);
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

        context["IReactiveSystem"] = () => {

            ReactiveSubSystemSpy entityCollectorSubSystem = null;
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

                entityCollectorSubSystem = new ReactiveSubSystemSpy(entityCollector);
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

        context["filter entities"] = () => {

            it["filters entities"] = () => {
                var filterEntitiesSystem = new ReactiveFilterEntitiesSubSystemSpy(_pools.test.CreateEntityCollector(_matcherAB),
                                                    e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                reactiveSystem = new ReactiveSystem(filterEntitiesSystem, _pools);

                _pools.test.CreateEntity()
                                               .AddComponentA()
                                               .AddComponentC();

                var eAB1 = _pools.test.CreateEntity()
                                .AddComponentB()
                                .AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _pools.test.CreateEntity()
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
                clearSubSystem = new ClearReactiveSubSystemSpy(_pools.test.CreateEntityCollector(_matcherAB));
                reactiveSystem = new ReactiveSystem(clearSubSystem, _pools);
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
