namespace Entitas {
    public partial class Pool {
        public ISystem CreateInitializeExecuteSystemSpy() {
            return this.CreateSystem<InitializeExecuteSystemSpy>();
        }
    }
}