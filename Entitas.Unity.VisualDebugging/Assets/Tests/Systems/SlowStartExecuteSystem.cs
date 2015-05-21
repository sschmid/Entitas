using Entitas;
using System.Threading;

public class SlowStartExecuteSystem : IStartSystem, IExecuteSystem {
    public void Start() {
        Thread.Sleep(80);
    }

    public void Execute() {
        Thread.Sleep(5);
    }
}

