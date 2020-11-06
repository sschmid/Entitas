using System.Linq;
using MyNamespace;
using NSpec;
using Shouldly;

class describe_CustomEntityIndex : nspec {

    void when_indexing() {

        it["sets entity to multiple keys"] = () => {
            var ctx = new TestContext();
            var index = new CustomEntityIndex(ctx);

            var e = ctx.CreateEntity();
            e.AddPosition(2, 3);
            e.AddSize(2, 2);

            index.GetEntities(new IntVector2(2, 3)).Single().ShouldBeSameAs(e);
            index.GetEntities(new IntVector2(3, 3)).Single().ShouldBeSameAs(e);
            index.GetEntities(new IntVector2(2, 4)).Single().ShouldBeSameAs(e);
            index.GetEntities(new IntVector2(3, 4)).Single().ShouldBeSameAs(e);
        };
    }
}
