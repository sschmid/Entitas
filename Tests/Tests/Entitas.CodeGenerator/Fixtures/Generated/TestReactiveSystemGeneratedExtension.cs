namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestReactiveSystem() {
            return this.CreateSystem<TestReactiveSystem>();
        }
    }
}