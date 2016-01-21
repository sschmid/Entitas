namespace Entitas {
    public partial class Pool {
        public ISystem CreateTestMultiReactiveSystem() {
            return this.CreateSystem<TestMultiReactiveSystem>();
        }
    }
}