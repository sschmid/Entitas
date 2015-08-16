namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeInitializeExecuteSystem() {
            return this.CreateSystem<SomeInitializeExecuteSystem>();
        }
    }
}