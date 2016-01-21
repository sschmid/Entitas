namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeSystem() {
            return this.CreateSystem<SomeSystem>();
        }
    }
}