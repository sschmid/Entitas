namespace Entitas {
    public partial class Pool {
        public ISystem CreateSlowStartSystem() {
            return this.CreateSystem<SlowStartSystem>();
        }
    }
}