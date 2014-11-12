using NSpec;
using Entitas;

class describe_ReactiveEntitySystem : nspec {

    void when_created() {
        EntityRepository repo = null;
        ReactiveEntitySystem res = null;
        ReactiveSubSystemSpy subSystem = null;
        before = () => {
            repo = new EntityRepository(CID.NumComponents);
        };

        context["OnEntityAdded"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityAdded();
                res = new ReactiveEntitySystem(repo, subSystem);
            };

            it["does not execute its subsystem when no entities were collected"] = () => {
                res.Execute();
                subSystem.didExecute.should_be(0);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityRemoved"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityRemoved();
                res = new ReactiveEntitySystem(repo, subSystem);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                e.RemoveComponentA();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = repo.CreateEntity();
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
                var e = repo.CreateEntity();
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
                res = new ReactiveEntitySystem(repo, subSystem);
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes when removed"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.AddComponentB();
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                res = new ReactiveEntitySystem(repo, subSystem);
                e.RemoveComponentA();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["collects matching entities created or modified in the subsystem"] = () => {
                var sys = new EntityEmittingSubSystem(repo);
                res = new ReactiveEntitySystem(repo, sys);
                var e = repo.CreateEntity();
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
        return new ReactiveSubSystemSpy(allOfAB(), EntityCollectionEventType.OnEntityAdded);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityRemoved() {
        return new ReactiveSubSystemSpy(allOfAB(), EntityCollectionEventType.OnEntityRemoved);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAddedOrRemoved() {
        return new ReactiveSubSystemSpy(allOfAB(), EntityCollectionEventType.OnEntityAddedOrRemoved);
    }

    IEntityMatcher allOfAB() {
        return Matcher.AllOf(new [] {
            CID.ComponentA,
            CID.ComponentB
        });
    }
}

