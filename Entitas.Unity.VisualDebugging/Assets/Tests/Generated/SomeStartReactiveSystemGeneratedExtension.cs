namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeStartReactiveSystem() {
            return this.CreateSystem<SomeInitializeReactiveSystem>();
        }
    }
}