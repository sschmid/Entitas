namespace Entitas {
    public partial class Pool {
        public ISystem CreateSlowSystem() {
            return this.CreateSystem<SlowSystem>();
        }
    }
}