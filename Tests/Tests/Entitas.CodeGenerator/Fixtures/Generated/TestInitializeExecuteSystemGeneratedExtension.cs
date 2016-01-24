namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestInitializeExecuteSystem() {
            return this.CreateSystem<TestInitializeExecuteSystem>();
        }
    }
}