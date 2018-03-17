using System.Threading;
using Entitas;

public sealed class TestJobSystem : JobSystem<TestEntity> {

    public TestJobSystem(TestContext context, int threads) :
        base(context.GetGroup(TestMatcher.NameAge), threads) {
    }

    protected override void Execute(TestEntity entity) {
        entity.nameAge.name += "-Processed";
        entity.nameAge.age = Thread.CurrentThread.ManagedThreadId;
    }
}
