namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestSystem() {
            return this.CreateSystem<TestSystem>();
        }
    }
}