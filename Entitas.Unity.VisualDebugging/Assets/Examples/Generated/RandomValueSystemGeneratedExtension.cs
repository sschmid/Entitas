namespace Entitas {
    public partial class Pool {
        public ISystem CreateRandomValueSystem() {
            return this.CreateSystem<RandomValueSystem>();
        }
    }
}