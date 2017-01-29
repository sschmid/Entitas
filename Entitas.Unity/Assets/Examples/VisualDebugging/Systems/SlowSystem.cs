using Entitas.Api;
using System.Threading;

public class SlowSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(4);
    }
}
