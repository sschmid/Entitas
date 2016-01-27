namespace Entitas {
    public partial class Pool {
        public ISystem CreateFastSystem() {
            return this.CreateSystem<FastSystem>();
        }
    }
}