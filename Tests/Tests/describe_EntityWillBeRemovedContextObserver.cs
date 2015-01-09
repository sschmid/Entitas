using NSpec;
using Entitas;
using System.Linq;

class describe_EntityWillBeRemovedContextObserver : nspec {

    void when_created() {

        Context ctx = null;
        WillBeRemovedContextObserver observer = null;
        before = () => {
            ctx = new Context(CID.NumComponents);
        };

        it["throws when matcher has not exactely one index"] = expect<MatcherException>(() => {
            new WillBeRemovedContextObserver(ctx, Matcher.AllOf(CID.ComponentA, CID.ComponentB));
        });

        context["when observing"] = () => {
            before = () => {
                observer = new WillBeRemovedContextObserver(ctx, Matcher.AllOf(CID.ComponentA));
            };

            it["returns collected entities"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                var pair = ecp.First();
                pair.entity.should_be_same(e);
                pair.component.should_be_same(Component.A);
            };

            it["only returns matching collected entities"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                var e2 = ctx.CreateEntity();
                e2.AddComponentB();
                e.RemoveComponentA();
                e2.RemoveComponentB();

                var ecp = observer.collectedEntityComponentPairs;
                ecp.Count.should_be(1);
                var pair = ecp.First();
                pair.entity.should_be_same(e);
            };

            it["collects entites only once"] = () => {
                var e = ctx.CreateEntity();
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
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                observer.Deactivate();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["doesn't collect entities when deactivated"] = () => {
                observer.Deactivate();
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();
                observer.collectedEntityComponentPairs.should_be_empty();
            };

            it["continues collecting when activated"] = () => {
                observer.Deactivate();
                var e1 = ctx.CreateEntity();
                e1.AddComponentA();
                e1.RemoveComponentA();

                observer.Activate();

                var e2 = ctx.CreateEntity();
                e2.AddComponentA();
                e2.RemoveComponentA();

                var ecp = observer.collectedEntityComponentPairs;
                var pair = ecp.First();
                pair.entity.should_be_same(e2);
            };

            it["clears collected entites"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.RemoveComponentA();

                observer.ClearCollectedEntites();
                observer.collectedEntityComponentPairs.should_be_empty();
            };
        };
    }
}

