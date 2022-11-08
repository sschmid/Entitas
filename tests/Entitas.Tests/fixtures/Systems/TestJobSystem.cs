using System;
using System.Threading;
using Entitas;

public sealed class TestJobSystem : JobSystem<Test1Entity> {

    public Exception exception;

    public TestJobSystem(Test1Context context, int threads) :
        base(context.GetGroup(Test1Matcher.NameAge), threads) {
    }

    protected override void Execute(Test1Entity entity) {
        if (exception != null) {
            throw exception;
        }

        entity.nameAge.name += "-Processed";
        entity.nameAge.age = Thread.CurrentThread.ManagedThreadId;
    }
}
