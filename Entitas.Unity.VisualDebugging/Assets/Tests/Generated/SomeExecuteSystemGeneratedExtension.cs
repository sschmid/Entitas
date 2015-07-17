namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeExecuteSystem() {
            return this.CreateSystem<SomeExecuteSystem>();
        }
    }
}