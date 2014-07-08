using NSpec;
using Entitas;

class describe_ReactiveEntitySystem : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository(CP.NumComponents);
    }

    void when_created() {
        ReactiveEntitySystem res = null;
        ReactiveSubSystemSpy subSystem = null;

        context["OnEntityAdded"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityAdded();
                res = new ReactiveEntitySystem(_repo, subSystem);
            };

            it["does not execute its subsystem when no entities were collected"] = () => {
                res.Execute();
                subSystem.didExecute.should_be(0);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = createEAB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = createEAB();
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = createEntity();
                addComponentA(e);
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityRemoved"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityRemoved();
                res = new ReactiveEntitySystem(_repo, subSystem);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = createEAB();
                removeComponentA(e);
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = createEAB();
                removeComponentA(e);
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = createEntity();
                addComponentA(e);
                addComponentB(e);
                addComponentC(e);
                removeComponentC(e);
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityAddedOrRemoved"] = () => {
            it["executes when added"] = () => {
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                res = new ReactiveEntitySystem(_repo, subSystem);
                var e = createEAB();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes when removed"] = () => {
                var e = createEAB();
                subSystem = getSubSystemSypWithOnEntityAddedOrRemoved();
                res = new ReactiveEntitySystem(_repo, subSystem);
                removeComponentA(e);
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["collects matching entities created or modified in the subsystem"] = () => {
                var sys = new EntityEmittingSubSystem(_repo);
                res = new ReactiveEntitySystem(_repo, sys);
                var e = createEntity();
                addComponentA(e);
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
        return EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        });
    }

    Entity createEntity() {
        return _repo.CreateEntity();
    }

    void addComponentA(Entity entity) {
        entity.AddComponent(CP.ComponentA, new ComponentA());
    }

    void addComponentB(Entity entity) {
        entity.AddComponent(CP.ComponentB, new ComponentB());
    }

    void addComponentC(Entity entity) {
        entity.AddComponent(CP.ComponentC, new ComponentC());
    }

    Entity createEAB() {
        var e = createEntity();
        addComponentA(e);
        addComponentB(e);
        return e;
    }

    void removeComponentA(Entity entity) {
        entity.RemoveComponent(CP.ComponentA);
    }

    void removeComponentC(Entity entity) {
        entity.RemoveComponent(CP.ComponentC);
    }
}

