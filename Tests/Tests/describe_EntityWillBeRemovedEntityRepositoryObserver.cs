using NSpec;
using Entitas;

class describe_EntityWillBeRemovedEntityRepositoryObserver : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository(CP.NumComponents);
    }

    void when_created() {

        EntityWillBeRemovedEntityRepositoryObserver observer = null;

        context["when observing"] = () => {
            before = () => {
                observer = new EntityWillBeRemovedEntityRepositoryObserver(_repo, CP.ComponentA);
            };

            it["returns collected entities"] = () => {
                var e = createEntity();
                var componentA = new ComponentA();
                e.AddComponent(CP.ComponentA, componentA);
                removeComponentA(e);

                var es = observer.collectedEntities;
                es.Count.should_be(1);
                es[0].should_be_same(e);

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                ecp[0].entity.should_be_same(e);
                ecp[0].component.should_be_same(componentA);
            };

            it["only returns matching collected entities"] = () => {
                var e = createEntity();
                addComponentA(e);
                var e2 = createEntity();
                addComponentB(e2);
                removeComponentA(e);
                removeComponentB(e2);

                var es = observer.collectedEntities;
                es.Count.should_be(1);
                es[0].should_be_same(e);

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                ecp[0].entity.should_be_same(e);
            };

            it["collects entites only once"] = () => {
                var e = createEntity();
                addComponentA(e);
                removeComponentA(e);
                addComponentA(e);
                removeComponentA(e);

                var es = observer.collectedEntities;
                es.Count.should_be(1);
                es[0].should_be_same(e);

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
            };

            it["returns empty list when no entities were collected"] = () => {
                observer.collectedEntities.should_be_empty();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["clears collected entities on deactivation"] = () => {
                var e = createEntity();
                addComponentA(e);
                removeComponentA(e);

                observer.Deactivate();
                observer.collectedEntities.should_be_empty();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = createEntity();
                addComponentA(e);
                removeComponentA(e);
                observer.collectedEntities.should_be_empty();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = createEntity();
                addComponentA(e1);
                removeComponentA(e1);

                observer.Activate();

                var e2 = createEntity();
                addComponentA(e2);
                removeComponentA(e2);

                var es = observer.collectedEntities;
                es.Count.should_be(1);
                es.should_contain(e2);

                var ecp = observer.collectedEntityComponentPairs;
                ecp[0].entity.should_be_same(e2);
            };

            it["clears collected entites"] = () => {
                var e = createEntity();
                addComponentA(e);
                removeComponentA(e);

                observer.ClearCollectedEntites();
                observer.collectedEntities.should_be_empty();
                observer.collectedEntityComponentPairs.should_be_empty();
            };
        };
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

    void removeComponentA(Entity entity) {
        entity.RemoveComponent(CP.ComponentA);
    }

    void removeComponentB(Entity entity) {
        entity.RemoveComponent(CP.ComponentB);
    }
}

