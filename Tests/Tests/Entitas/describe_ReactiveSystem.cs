using NSpec;
using Entitas;

class describe_ReactiveSystem : nspec {

    void when_created() {
        Pool pool = null;
        ReactiveSystem reactiveSystem = null;
        ReactiveSubSystemSpy subSystem = null;
        MultiReactiveSubSystemSpy multiSubSystem = null;
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

            it["executes when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                reactiveSystem.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
            };

            it["deactivates"] = () => {
                reactiveSystem.Deactivate();
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
            };

            it["activates"] = () => {
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
        };

        context["OnEntityRemoved"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityRemoved();
                reactiveSystem = new ReactiveSystem(pool, subSystem);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                reactiveSystem.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entities.Length.should_be(1);
                subSystem.entities.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
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

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.AddComponentC();
                e.RemoveComponentC();
                reactiveSystem.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entities.should_be_null();
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
                sys.entites.Length.should_be(1);
                reactiveSystem.Execute();
                sys.entites.Length.should_be(1);
            };
        };

        context["MultiReactiveSystem"] = () => {
            before = () => {
                var matchers = new IMatcher[] {
                    Matcher.AllOf(new [] { CID.ComponentA }),
                    Matcher.AllOf(new [] { CID.ComponentB })
                };
                var eventTypes = new [] {
                    GroupEventType.OnEntityAdded,
                    GroupEventType.OnEntityRemoved,
                };
                multiSubSystem = new MultiReactiveSubSystemSpy(matchers, eventTypes);
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

        context["ensure components matcher"] = () => {

            it["only passes in entites matching required matcher"] = () => {
                var ensureSubSystem = new ReactiveEnsureSubSystemSpy(
                    allOfAB(),
                    GroupEventType.OnEntityAdded,
                    Matcher.AllOf(new [] {
                        CID.ComponentA,
                        CID.ComponentB,
                        CID.ComponentC
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

                ensureSubSystem.didExecute.should_be(1);
                ensureSubSystem.entities.Length.should_be(1);
                ensureSubSystem.entities.should_contain(eABC);
            };

            it["only passes in entites matching required matcher (multi reactive)"] = () => {
                var matchers = new IMatcher[] {
                    Matcher.AllOf(new [] { CID.ComponentA }),
                    Matcher.AllOf(new [] { CID.ComponentB })
                };
                var eventTypes = new [] {
                    GroupEventType.OnEntityAdded,
                    GroupEventType.OnEntityRemoved,
                };

                var ensure = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB,
                    CID.ComponentC
                });

                var ensureSubSystem = new MultiReactiveEnsureSubSystemSpy(matchers, eventTypes, ensure);
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
        };

        context["exlude components"] = () => {
            it["only passes in entites not matching matcher"] = () => {
                var excludeSubSystem = new ReactiveExcludeSubSystemSpy(
                    allOfAB(),
                    GroupEventType.OnEntityAdded,
                    Matcher.AllOf(new [] { CID.ComponentC })
                );
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

            it["only passes in entites not matching required matcher (multi reactive)"] = () => {
                var matchers = new IMatcher[] {
                    Matcher.AllOf(new [] { CID.ComponentA }),
                    Matcher.AllOf(new [] { CID.ComponentB })
                };
                var eventTypes = new [] {
                    GroupEventType.OnEntityAdded,
                    GroupEventType.OnEntityRemoved,
                };

                var exclude = Matcher.AllOf(new [] {
                    CID.ComponentC
                });

                var excludeSubSystem = new MultiReactiveExcludeSubSystemSpy(matchers, eventTypes, exclude);
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
            it["only passes in entites"] = () => {
                var ensureExcludeSystem = new ReactiveEnsureExcludeSubSystemSpy(
                    allOfAB(),
                    GroupEventType.OnEntityAdded,
                    Matcher.AllOf(new [] { CID.ComponentA, CID.ComponentB }),
                    Matcher.AllOf(new [] { CID.ComponentC })
                );
                reactiveSystem = new ReactiveSystem(pool, ensureExcludeSystem);

                var eAB = pool.CreateEntity();
                eAB.AddComponentA();
                eAB.AddComponentB();
                var eAC = pool.CreateEntity();
                eAC.AddComponentA();
                eAC.AddComponentC();
                var eABC = pool.CreateEntity();
                eABC.AddComponentA();
                eABC.AddComponentB();
                eABC.AddComponentC();
                reactiveSystem.Execute();

                ensureExcludeSystem.didExecute.should_be(1);
                ensureExcludeSystem.entities.Length.should_be(1);
                ensureExcludeSystem.entities.should_contain(eAB);
            };
        };
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAdded() {
        return new ReactiveSubSystemSpy(allOfAB(), GroupEventType.OnEntityAdded);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityRemoved() {
        return new ReactiveSubSystemSpy(allOfAB(), GroupEventType.OnEntityRemoved);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAddedOrRemoved() {
        return new ReactiveSubSystemSpy(allOfAB(), GroupEventType.OnEntityAddedOrRemoved);
    }

    IMatcher allOfAB() {
        return Matcher.AllOf(new [] {
            CID.ComponentA,
            CID.ComponentB
        });
    }
}

