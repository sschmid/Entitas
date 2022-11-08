using System.Threading;
using UnityEngine;
using Entitas;

public class RandomDurationSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(Random.Range(0, 9));
    }
}
