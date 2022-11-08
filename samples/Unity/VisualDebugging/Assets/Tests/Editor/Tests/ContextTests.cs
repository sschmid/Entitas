using NUnit.Framework;
using Entitas;

[TestFixture]
class ContextTests {

    [SetUp]
    public void BeforeEach() {

    }

    [Test]
    public void ensures_same_deterministic_order_when_getting_entities_after_DestroyAllEntities() {
        var context = new Context<Entity>(1, () => new GameEntity());

        const int numEntities = 10;
        for (int i = 0; i < numEntities; i++) {
            context.CreateEntity();
        }

        var order1 = new int[numEntities];
        var entities1 = context.GetEntities();
        for (int i = 0; i < numEntities; i++) {
            order1[i] = entities1[i].creationIndex;
        }

        context.DestroyAllEntities();
        context.ResetCreationIndex();

        for (int i = 0; i < numEntities; i++) {
            context.CreateEntity();
        }

        var order2 = new int[numEntities];
        var entities2 = context.GetEntities();
        for (int i = 0; i < numEntities; i++) {
            order2[i] = entities2[i].creationIndex;
        }

        for (int i = 0; i < numEntities; i++) {
            var index1 = order1[i];
            var index2 = order2[i];

            Assert.AreEqual(index1, index2);
        }
    }
}
