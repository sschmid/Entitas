namespace Entitas {
    public partial class Pool {
        public ISystem CreateSomeStartSystem() {
            return this.CreateSystem<SomeStartSystem>();
        }
    }
}