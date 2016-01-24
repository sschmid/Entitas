namespace Entitas {
    public partial class Pool {
        public ISystem CreateExecuteSystemSpy() {
            return this.CreateSystem<ExecuteSystemSpy>();
        }
    }
}