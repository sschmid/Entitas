using NSpec;
using Entitas;

class describe_PoolObserver : nspec {

    void when_created() {

        Pool pool = null;
        PoolObserver observer = null;
        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observer = new PoolObserver(pool, Matcher.AllOf(new [] { CID.ComponentA }), GroupEventType.OnEntityAdded);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["only returns matching collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                var e2 = pool.CreateEntity();
                e2.AddComponentB();
                
                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e);
            };

            it["collects entites only once"] = () => {
                var e = pool.CreateEntity();
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
                var e = pool.CreateEntity();
                e.AddComponentA();

                observer.Deactivate();
                observer.collectedEntities.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = pool.CreateEntity();
                e.AddComponentA();
                observer.collectedEntities.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = pool.CreateEntity();
                e1.AddComponentA();

                observer.Activate();

                var e2 = pool.CreateEntity();
                e2.AddComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(1);
                entities.should_contain(e2);
            };

            it["clears collected entites"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();

                observer.ClearCollectedEntites();
                observer.collectedEntities.should_be_empty();
            };
        };

        context["when observing with eventType OnEntityRemoved"] = () => {
            before = () => {
                observer = new PoolObserver(pool, Matcher.AllOf(new [] { CID.ComponentA }), GroupEventType.OnEntityRemoved);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
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
                observer = new PoolObserver(pool, Matcher.AllOf(new [] { CID.ComponentA }), GroupEventType.OnEntityAddedOrRemoved);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
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

