using NSpec;
using Entitas;

class describe_EntityRepositoryObserver : nspec {

    void when_created() {

        EntityRepository repo = null;
        EntityRepositoryObserver observer = null;
        before = () => {
            repo = new EntityRepository(CID.NumComponents);
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(repo, Matcher.AllOf(new [] { CID.ComponentA }), EntityCollectionEventType.OnEntityAdded);
            };

            it["returns collected entities"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["only returns matching collected entities"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                var e2 = repo.CreateEntity();
                e2.AddComponentB();
                
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["collects entites only once"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();
                e.AddComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["returns empty list when no entities were collected"] = () => {
                observer.collectedEntities.should_be_empty();
            };

            it["clears collected entities on deactivation"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();

                observer.Deactivate();
                observer.collectedEntities.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = repo.CreateEntity();
                e.AddComponentA();
                observer.collectedEntities.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = repo.CreateEntity();
                e1.AddComponentA();

                observer.Activate();

                var e2 = repo.CreateEntity();
                e2.AddComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e2);
            };

            it["clears collected entites"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();

                observer.ClearCollectedEntites();
                observer.collectedEntities.should_be_empty();
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(repo, Matcher.AllOf(new [] { CID.ComponentA }), EntityCollectionEventType.OnEntityRemoved);
            };

            it["returns collected entities"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                observer.collectedEntities.should_be_empty();

                e.RemoveComponentA();
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };

        context["when observing with eventType OnEntityAddedOrRemoved"] = () => {
            before = () => {
                observer = new EntityRepositoryObserver(repo, Matcher.AllOf(new [] { CID.ComponentA }), EntityCollectionEventType.OnEntityAddedOrRemoved);
            };

            it["returns collected entities"] = () => {
                var e = repo.CreateEntity();
                e.AddComponentA();
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
                observer.ClearCollectedEntites();

                e.RemoveComponentA();
                entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };
        };
    }
}

