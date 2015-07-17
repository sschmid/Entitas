namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeReactiveSystem() {
            return this.CreateSystem<SomeReactiveSystem>();
        }
    }
}