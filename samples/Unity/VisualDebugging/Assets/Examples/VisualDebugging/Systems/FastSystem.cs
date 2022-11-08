using System.Threading;
using Entitas;

public class FastSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(1);
    }
}
