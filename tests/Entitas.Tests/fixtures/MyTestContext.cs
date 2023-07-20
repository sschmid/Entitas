using Entitas;

public sealed class MyTest1Context : Context<TestEntity> {

    public MyTest1Context() : base(CID.TotalComponents, () => new TestEntity()) {
    }

    public MyTest1Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
        : base(totalComponents, startCreationIndex, contextInfo, SafeAERC.Delegate, () => new TestEntity()) {
    }
}
