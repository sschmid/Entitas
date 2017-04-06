using System.Threading;
using Entitas.Core;

public class SlowInitializeSystem : IInitializeSystem {

    public void Initialize() {
        Thread.Sleep(30);
    }
}
