namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeInitializeSystem() {
            return this.CreateSystem<SomeInitializeSystem>();
        }
    }
}