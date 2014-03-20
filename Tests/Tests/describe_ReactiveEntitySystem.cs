using NSpec;
using Entitas;

class describe_ReactiveEntitySystem : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository();
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAdded() {
        return new ReactiveSubSystemSpy(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB)
        }), EntityCollectionEventType.OnEntityAdded);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityRemoved() {
        return new ReactiveSubSystemSpy(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB)
        }), EntityCollectionEventType.OnEntityRemoved);
    }

    ReactiveSubSystemSpy getSubSystemSypWithOnEntityAddedSafe() {
        return new ReactiveSubSystemSpy(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB)
        }), EntityCollectionEventType.OnEntityAddedSafe);
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
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
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
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                e.RemoveComponent(typeof(ComponentA));
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["executes only once when triggeringMatcher matches"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                e.RemoveComponent(typeof(ComponentA));
                res.Execute();
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when triggeringMatcher doesn't match"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                e.AddComponent(new ComponentC());
                e.RemoveComponent(typeof(ComponentC));
                res.Execute();
                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };
        };

        context["OnEntityAddedSafe"] = () => {
            before = () => {
                subSystem = getSubSystemSypWithOnEntityAddedSafe();
                res = new ReactiveEntitySystem(_repo, subSystem);
            };

            it["executes when triggeringMatcher matches"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };

            it["doesn't execute when entity doesn't match anymore"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                e.RemoveComponent(typeof(ComponentA));
                res.Execute();

                subSystem.didExecute.should_be(0);
                subSystem.entites.should_be_null();
            };

            it["doesn't execute when replaced component is not in triggering matcher"] = () => {
                var e = _repo.CreateEntity();
                e.AddComponent(new ComponentA());
                e.AddComponent(new ComponentB());
                e.AddComponent(new ComponentC());
                res.Execute();
                e.ReplaceComponent(new ComponentC());
                res.Execute();

                subSystem.didExecute.should_be(1);
                subSystem.entites.Length.should_be(1);
                subSystem.entites.should_contain(e);
            };
        };
    }
}

