using System.Threading;
using Entitas;

public class SlowSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(4);
    }
}
