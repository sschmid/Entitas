using Entitas;

public sealed class MyTestContext : Context<TestEntity> {

    public MyTestContext() : base(CID.TotalComponents, () => new TestEntity()) {
    }

    public MyTestContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
        : base(totalComponents, startCreationIndex, contextInfo, (entity) => new SafeAERC(entity), () => new TestEntity()) {
    }
}
