using Entitas;
using System.Threading;
using UnityEngine;

public class RandomDurationSystem : IExecuteSystem {

    public void Execute() {
        Thread.Sleep(Random.Range(0, 9));
    }
}
