namespace Entitas {
    public partial class Pool {
        public ISystem CreateSlowStartExecuteSystem() {
            return this.CreateSystem<SlowStartExecuteSystem>();
        }
    }
}