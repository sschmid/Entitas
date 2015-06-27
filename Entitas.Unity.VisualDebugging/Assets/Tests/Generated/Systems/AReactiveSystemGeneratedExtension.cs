namespace Entitas {
    public partial class Pool {
        public ISystem CreateAReactiveSystem() {
            return this.CreateSystem<AReactiveSystem>();
        }
    }
}