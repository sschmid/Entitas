using NUnit.Framework;
using Entitas;

[TestFixtureAttribute]
class PoolTests {

	[SetUp]
	public void BeforeEach() {

	}

	[Test]
	public void ensures_same_deterministic_order_when_getting_entities_after_DestroyAllEntities() {
		var pool = new Pool(1);

		const int numEntities = 10;
		for (int i = 0; i < numEntities; i++) {
			pool.CreateEntity();
		}

		var order1 = new int[numEntities];
		var entities1 = pool.GetEntities();
		for (int i = 0; i < numEntities; i++) {
			order1[i] = entities1[i].creationIndex;
		}

		pool.DestroyAllEntities();
		pool.ResetCreationIndex();

		for (int i = 0; i < numEntities; i++) {
			pool.CreateEntity();
		}

		var order2 = new int[numEntities];
		var entities2 = pool.GetEntities();
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
