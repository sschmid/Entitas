namespace Entitas {
    public partial class Pool {
        public ISystem CreateSlowInitializeExecuteSystem() {
            return this.CreateSystem<SlowInitializeExecuteSystem>();
        }
    }
}