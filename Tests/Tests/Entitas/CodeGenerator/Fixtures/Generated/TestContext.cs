namespace Entitas {

    public sealed partial class TestContext : Context<TestEntity> {

        public TestContext() : base(CID.TotalComponents) {
        }
    }
}