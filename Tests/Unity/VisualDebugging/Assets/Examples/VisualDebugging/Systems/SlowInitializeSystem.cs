using System.Threading;
using Entitas;

public class SlowInitializeSystem : IInitializeSystem {

    public void Initialize() {
        Thread.Sleep(30);
    }
}
