using System;
using System.Threading;
using Entitas;

public sealed class TestJobSystem : JobSystem<TestEntity> {

    public Exception exception;

    public TestJobSystem(TestContext context, int threads) :
        base(context.GetGroup(TestUserMatcher.User), threads) {
    }

    protected override void Execute(TestEntity entity) {
        if (exception != null) {
            throw exception;
        }

        var user = entity.GetUser();
        user.Name += "-Processed";
        user.Age = Thread.CurrentThread.ManagedThreadId;
    }
}
