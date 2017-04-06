using System.Threading;
using Entitas.Core;

public class SlowSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(4);
    }
}
