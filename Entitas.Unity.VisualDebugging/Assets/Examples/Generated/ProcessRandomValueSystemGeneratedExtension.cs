namespace Entitas {
    public partial class Pool {
        public ISystem CreateProcessRandomValueSystem() {
            return this.CreateSystem<ProcessRandomValueSystem>();
        }
    }
}