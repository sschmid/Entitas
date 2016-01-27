namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeInitializeReactiveSystem() {
            return this.CreateSystem<SomeInitializeReactiveSystem>();
        }
    }
}