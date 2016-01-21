namespace Entitas {
    public partial class Pool {
        public ISystem CreateInitializeSystemSpy() {
            return this.CreateSystem<InitializeSystemSpy>();
        }
    }
}