using System.Threading;
using Entitas.Core;

public class FastSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(1);
    }
}
