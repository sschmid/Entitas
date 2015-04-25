using NSpec;
using Entitas;

class describe_ReactiveSystem : nspec {

    void when_created() {
        Pool pool = null;
        ReactiveSystem res = null;
        ReactiveSubSystemSpy subSystem = null;
        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        context["OnEntityAdded"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityAdded();
                res = new ReactiveSystem(pool, subSystem);
            };

            it["does not execute its subsystem when no entities were collected"] = () => {
                res.Execute();
                subSystem.didExecute.should_be(0);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityRemoved"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityRemoved();
                res = new ReactiveSystem(pool, subSystem);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.AddComponentC();
                e.RemoveComponentC();
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {
            it["executes when added"] = () => {
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                res = new ReactiveSystem(pool, subSystem);
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes when removed"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                res = new ReactiveSystem(pool, subSystem);
                e.RemoveComponentA();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["collects matching entities created or modified in the subsystem"] = () => {
                var sys = new EntityEmittingSubSystem(pool);
                res = new ReactiveSystem(pool, sys);
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();
                sys.entites.Length.should_be(1);
                res.Execute();
                sys.entites.Length.should_be(1);
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

