using System;
using System.Threading;
using Entitas;

public sealed class TestParallelSystem : ParallelSystem<TestEntity>
{
    public Exception Exception;

    public TestParallelSystem(TestContext context) : base(context.GetGroup(TestUserMatcher.User)) { }

    protected override void Execute(TestEntity entity)
    {
        if (Exception != null)
        {
            throw Exception;
        }

        var user = entity.GetUser();
        user.Name += "-Processed";
        user.Age = Thread.CurrentThread.ManagedThreadId;
    }
}
