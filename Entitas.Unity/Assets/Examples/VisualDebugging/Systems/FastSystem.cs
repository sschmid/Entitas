using Entitas;
using System.Threading;

public class FastSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(1);
    }
}

