using NSpec;
using Entitas;

class describe_GroupObserver : nspec {

    void when_created() {

        Pool pool = null;
        Group group = null;
        GroupObserver observer = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
            group = pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
        };

        context["when observing with eventType OnEntityAdded"] = () => {
            before = () => {
                observer = new GroupObserver(group, GroupEventType.OnEntityAdded);
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
                observer = new GroupObserver(group, GroupEventType.OnEntityRemoved);
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
                observer = new GroupObserver(group, GroupEventType.OnEntityAddedOrRemoved);
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

        context["when observing with eventType OnEntityAddedStrict"] = () => {
            before = () => {
                observer = new GroupObserver(group, GroupEventType.OnEntityAddedStrict);
            };

            it["returns collected entities and ensures entities still match"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                var entities = observer.collectedEntities;
                entities.Count.should_be(0);
            };
        };

        context["when observing with eventType OnEntityRemovedStrict"] = () => {
            before = () => {
                observer = new GroupObserver(group, GroupEventType.OnEntityRemovedStrict);
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                observer.collectedEntities.should_be_empty();
                e.RemoveComponentA();
                e.AddComponentA();
                var entities = observer.collectedEntities;
                entities.Count.should_be(0);
            };
        };
    }
}

