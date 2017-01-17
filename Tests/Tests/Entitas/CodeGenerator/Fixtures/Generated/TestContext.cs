namespace Entitas {

    public sealed partial class TestContext : XXXContext<TestEntity> {

        public TestContext() : base(CID.TotalComponents) {
        }
    }
}