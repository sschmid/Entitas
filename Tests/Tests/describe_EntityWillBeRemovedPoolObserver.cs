using NSpec;
using Entitas;
using System.Linq;

class describe_EntityWillBeRemovedPoolObserver : nspec {

    void when_created() {

        Pool pool = null;
        WillBeRemovedPoolObserver observer = null;
        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        it["throws when matcher has not exactely one index"] = expect<MatcherException>(() => {
            new WillBeRemovedPoolObserver(pool, Matcher.AllOf(CID.ComponentA, CID.ComponentB));
        });

        context["when observing"] = () => {
            before = () => {
                observer = new WillBeRemovedPoolObserver(pool, Matcher.AllOf(CID.ComponentA));
            };

            it["returns collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                var pair = ecp.First();
                pair.entity.should_be_same(e);
                pair.component.should_be_same(Component.A);
            };

            it["only returns matching collected entities"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                var e2 = pool.CreateEntity();
                e2.AddComponentB();
                e.RemoveComponentA();
                e2.RemoveComponentB();

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                var pair = ecp.First();
                pair.entity.should_be_same(e);
            };

            it["collects entites only once"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();
                e.AddComponentA();
                e.RemoveComponentA();

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
            };

            it["returns empty list when no entities were collected"] = () => {
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["clears collected entities on deactivation"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                observer.Deactivate();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = pool.CreateEntity();
                e1.AddComponentA();
                e1.RemoveComponentA();

                observer.Activate();

                var e2 = pool.CreateEntity();
                e2.AddComponentA();
                e2.RemoveComponentA();

                var ecp = observer.collectedEntityComponentPairs;
                var pair = ecp.First();
                pair.entity.should_be_same(e2);
            };

            it["clears collected entites"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                observer.ClearCollectedEntites();
                observer.collectedEntityComponentPairs.should_be_empty();
            };
        };
    }
}

