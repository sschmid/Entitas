namespace Entitas {
    public partial class Pool {
        public ISystem CreateRandomDurationSystem() {
            return this.CreateSystem<RandomDurationSystem>();
        }
    }
}