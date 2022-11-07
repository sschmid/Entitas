using System.Threading;
using Entitas;

public class SlowInitializeExecuteSystem : IInitializeSystem, IExecuteSystem {

    public void Initialize() {
        Thread.Sleep(10);
    }

    public void Execute() {
        Thread.Sleep(5);
    }
}
