using NSpec;
using Entitas;

class describe_EntityRepositoryObserver : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository(CP.NumComponents);
    }

    void when_created() {

        EntityRepositoryObserver observer = null;

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(_repo, EntityCollectionEventType.OnEntityAdded, EntityMatcher.AllOf(new [] { CP.ComponentA }));
            };

            it["returns collected entities"] = () => {
                var e = createEntity();
                addComponentA(e);

                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["only returns matching collected entities"] = () => {
                var e = createEntity();
                addComponentA(e);
                var e2 = createEntity();
                addComponentB(e2);
                
                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["collects entites only once"] = () => {
                var e = createEntity();
                addComponentA(e);
                removeComponentA(e);
                addComponentA(e);

                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["returns empty list when no entities were collected"] = () => {
                observer.collectedEntites.should_be_empty();
            };

            it["clears collected entities on deactivation"] = () => {
                var e = createEntity();
                addComponentA(e);

                observer.Deactivate();
                observer.collectedEntites.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = createEntity();
                addComponentA(e);
                observer.collectedEntites.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = createEntity();
                addComponentA(e1);

                observer.Activate();

                var e2 = createEntity();
                addComponentA(e2);

                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e2);
            };

            it["clears collected entites"] = () => {
                var e = createEntity();
                addComponentA(e);

                observer.ClearCollectedEntites();
                observer.collectedEntites.should_be_empty();
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(_repo, EntityCollectionEventType.OnEntityRemoved, EntityMatcher.AllOf(new [] { CP.ComponentA }));
            };

            it["returns collected entities"] = () => {
                var e = createEntity();
                addComponentA(e);
                observer.collectedEntites.should_be_empty();

                removeComponentA(e);
                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(_repo, EntityCollectionEventType.OnEntityAddedOrRemoved, EntityMatcher.AllOf(new [] { CP.ComponentA }));
            };

            it["returns collected entities"] = () => {
                var e = createEntity();
                addComponentA(e);
                var entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);

                removeComponentA(e);
                entities = observer.collectedEntites;
                entities.Count.should_be(1);
                entities.should_contain(e);
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
}

