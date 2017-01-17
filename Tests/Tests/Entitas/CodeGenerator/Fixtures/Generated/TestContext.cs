namespace Entitas {

    public sealed partial class TestContext : Context<TestEntity> {

        public TestContext() : base(CID.TotalComponents) {
        }

        public TestContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
            : base(totalComponents, startCreationIndex, contextInfo) {
        }
    }
}