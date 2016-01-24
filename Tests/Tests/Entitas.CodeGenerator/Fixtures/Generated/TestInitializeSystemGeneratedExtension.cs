namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestInitializeSystem() {
            return this.CreateSystem<TestInitializeSystem>();
        }
    }
}