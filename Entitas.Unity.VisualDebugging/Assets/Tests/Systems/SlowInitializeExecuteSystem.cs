using Entitas;
using System.Threading;

public class SlowInitializeExecuteSystem : IInitializeSystem, IExecuteSystem {
    public void Initialize() {
        Thread.Sleep(80);
    }

    public void Execute() {
        Thread.Sleep(5);
    }
}

