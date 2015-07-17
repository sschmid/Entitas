namespace Entitas {
    public partial class Pool {
        public ISystem CreateCollectReactiveSystem() {
            return this.CreateSystem<CollectReactiveSystem>();
        }
    }
}