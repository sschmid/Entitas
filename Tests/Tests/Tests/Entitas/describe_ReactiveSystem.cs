using Entitas;
using NSpec;

class describe_ReactiveSystem : nspec {

    readonly IMatcher<TestEntity> _matcherAB = Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);

    static void assertEntities(IReactiveSystemSpy system, TestEntity entity, int didExecute = 1) {
        if (entity == null) {
            system.didExecute.should_be(0);
            system.entities.should_be_null();

        } else {
            system.didExecute.should_be(didExecute);
            system.entities.Length.should_be(1);
            system.entities.should_contain(entity);
        }
    }

    MyTestContext _context;

    TestEntity createEntityAB() {
        return _context.CreateEntity()
            .AddComponentA()
            .AddComponentB();
    }

    TestEntity createEntityAC() {
        return _context.CreateEntity()
            .AddComponentA()
            .AddComponentC();
    }

    TestEntity createEntityABC() {
        return _context.CreateEntity()
            .AddComponentA()
            .AddComponentB()
            .AddComponentC();
    }

    void when_created() {

        ReactiveSystemSpy system = null;

        before = () => {
            _context = new MyTestContext();
        };

        context["OnEntityAdded"] = () => {

            before = () => {
                system = new ReactiveSystemSpy(_context.CreateCollector<TestEntity>(_matcherAB));
            };

            it["does not execute when no entities were collected"] = () => {
                system.Execute();
                assertEntities(system, null);
            };

            it["executes when triggered"] = () => {
                var e = createEntityAB();
                system.Execute();
                assertEntities(system, e);
            };

            it["executes only once when triggered"] = () => {
                var e = createEntityAB();
                system.Execute();
                system.Execute();
                assertEntities(system, e);
            };

            it["retains and releases collected entities"] = () => {
                var e = createEntityAB();
                var retainCount = e.retainCount;
                system.Execute();
                retainCount.should_be(3); // retained by context, group and collector
                e.retainCount.should_be(2); // retained by context and group
            };

            it["collects changed entities in execute"] = () => {
                var e = createEntityAB();
                system.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                system.Execute();
                system.Execute();
                assertEntities(system, e, 2);
            };

            it["collects created entities in execute"] = () => {
                var e1 = createEntityAB();
                TestEntity e2 = null;
                system.executeAction = entities => {
                    if (e2 == null) {
                        e2 = createEntityAB();
                    }
                };

                system.Execute();
                assertEntities(system, e1);

                system.Execute();
                assertEntities(system, e2, 2);
            };

            it["doesn't execute when not triggered"] = () => {
                _context.CreateEntity().AddComponentA();
                system.Execute();
                assertEntities(system, null);
            };

            it["deactivates and will not trigger"] = () => {
                system.Deactivate();
                createEntityAB();
                system.Execute();
                assertEntities(system, null);
            };

            it["activates and will trigger again"] = () => {
                system.Deactivate();
                system.Activate();
                var e = createEntityAB();
                system.Execute();
                assertEntities(system, e);
            };

            it["clears"] = () => {
                createEntityAB();
                system.Clear();
                system.Execute();
                assertEntities(system, null);
            };

            it["can ToString"] = () => {
                system.ToString().should_be("ReactiveSystem(ReactiveSystemSpy)");
            };
        };

        context["OnEntityRemoved"] = () => {

            before = () => {
                system = new ReactiveSystemSpy(_context.CreateCollector(_matcherAB, GroupEvent.Removed));
            };

            it["executes when triggered"] = () => {
                var e = createEntityAB()
                    .RemoveComponentA();

                system.Execute();
                assertEntities(system, e);
            };

            it["executes only once when triggered"] = () => {
                var e = createEntityAB()
                    .RemoveComponentA();

                system.Execute();
                system.Execute();
                assertEntities(system, e);
            };

            it["doesn't execute when not triggered"] = () => {
                createEntityAB()
                    .AddComponentC()
                    .RemoveComponentC();

                system.Execute();
                assertEntities(system, null);
            };

            it["retains entities until execute completed"] = () => {
                var e = createEntityAB();
                var didExecute = 0;
                system.executeAction = entities => {
                    didExecute += 1;
                    entities[0].retainCount.should_be(1);
                };

                _context.DestroyEntity(e);
                system.Execute();
                didExecute.should_be(1);
                e.retainCount.should_be(0);
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {

            before = () => {
                system = new ReactiveSystemSpy(_context.CreateCollector(_matcherAB, GroupEvent.AddedOrRemoved));
            };

            it["executes when added"] = () => {
                var e = createEntityAB();
                system.Execute();
                assertEntities(system, e);
            };

            it["executes when removed"] = () => {
                var e = createEntityAB();
                system.Execute();
                e.RemoveComponentA();
                system.Execute();
                assertEntities(system, e, 2);
            };
        };

        context["multiple contexts"] = () => {

            IContext<TestEntity> context1 = null;
            IContext<TestEntity> context2 = null;

            before = () => {
                context1 = new MyTestContext();
                context2 = new MyTestContext();

                var groupA = context1.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                var groupB = context2.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

                var groups = new [] { groupA, groupB };
                var groupEvents = new [] {
                    GroupEvent.Added,
                    GroupEvent.Removed
                };
                var collector = new Collector<TestEntity>(groups, groupEvents);

                system = new ReactiveSystemSpy(collector);
            };

            it["executes when a triggered by collector"] = () => {
                var eA1 = context1.CreateEntity().AddComponentA();
                context2.CreateEntity().AddComponentA();

                var eB1 = context1.CreateEntity().AddComponentB();
                var eB2 = context2.CreateEntity().AddComponentB();

                system.Execute();
                assertEntities(system, eA1);

                eB1.RemoveComponentB();
                eB2.RemoveComponentB();
                system.Execute();
                assertEntities(system, eB2, 2);
            };
        };

        context["filter entities"] = () => {

            it["filters entities"] = () => {
                system = new ReactiveSystemSpy(_context.CreateCollector(_matcherAB),
                                               e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42);

                _context.CreateEntity()
                                   .AddComponentA()
                                   .AddComponentC();

                var eAB1 = _context.CreateEntity();
                eAB1.AddComponentB();
                eAB1.AddComponent(CID.ComponentA, new NameAgeComponent { age = 10 });

                var eAB2 = _context.CreateEntity();
                eAB2.AddComponentB();
                eAB2.AddComponent(CID.ComponentA, new NameAgeComponent { age = 50 });

                var didExecute = 0;
                system.executeAction = entities => {
                    didExecute += 1;
                    eAB2.retainCount.should_be(3); // retained by context, group and collector
                };

                system.Execute();
                didExecute.should_be(1);

                system.Execute();

                system.entities.Length.should_be(1);
                system.entities[0].should_be_same(eAB2);

                eAB1.retainCount.should_be(2); // retained by context and group
                eAB2.retainCount.should_be(2);
            };
        };

        context["clear"] = () => {

            it["clears reactive system after execute"] = () => {
                system = new ReactiveSystemSpy(_context.CreateCollector(_matcherAB));

                system.executeAction = entities => {
                    entities[0].ReplaceComponentA(Component.A);
                };

                var e = createEntityAB();
                system.Execute();
                system.Clear();
                system.Execute();
                assertEntities(system, e);
            };
        };
    }
}
