using Entitas;
using System.Threading;

public class SlowStartSystem : IStartSystem {
    public void Start() {
        Thread.Sleep(100);
    }
}

