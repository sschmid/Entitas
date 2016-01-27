namespace Entitas {
    public partial class Pool {
        public ISystem CreateSlowInitializeSystem() {
            return this.CreateSystem<SlowInitializeSystem>();
        }
    }
}