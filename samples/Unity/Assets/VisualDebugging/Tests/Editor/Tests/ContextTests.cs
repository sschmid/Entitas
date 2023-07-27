using System.Linq;
using NUnit.Framework;
using Entitas;

[TestFixture]
class ContextTests
{
    [Test]
    public void EnsuresSameDeterministicOrderWhenGettingEntitiesAfterDestroyAllEntities()
    {
        ContextInitialization.InitializeAllContexts();
        var gameContext = new Context<Entity>(1, () => new Game.Entity());

        const int numEntities = 10;

        for (var i = 0; i < numEntities; i++)
            gameContext.CreateEntity();

        var order1 = gameContext.GetEntities().Select(entity => entity.Id).ToArray();

        gameContext.Reset();

        for (var i = 0; i < numEntities; i++)
            gameContext.CreateEntity();

        var order2 = gameContext.GetEntities().Select(entity => entity.Id).ToArray();

        for (var i = 0; i < numEntities; i++)
        {
            var index1 = order1[i];
            var index2 = order2[i];

            Assert.AreEqual(index1, index2);
        }
    }
}
