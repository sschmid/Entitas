using Entitas;
using System.Threading;

public class SlowInitializeSystem : IInitializeSystem {

    public void Initialize() {
        Thread.Sleep(30);
    }
}
