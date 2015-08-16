namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeStartExecuteSystem() {
            return this.CreateSystem<SomeInitializeExecuteSystem>();
        }
    }
}