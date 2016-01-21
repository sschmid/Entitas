namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestExecuteSystem() {
            return this.CreateSystem<TestExecuteSystem>();
        }
    }
}