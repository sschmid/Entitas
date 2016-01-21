namespace Entitas {
    public partial class Pool {
        public ISystem CreateNamespaceSystem() {
            return this.CreateSystem<Tests.NamespaceSystem>();
        }
    }
}