using Entitas;
using UnityEngine;

public class RandomValueSystem : IExecuteSystem {

    readonly Pool _pool;

    public RandomValueSystem(Pools pools) {
        _pool = pools.visualDebugging;
    }

    public void Execute() {
        _pool.CreateEntity().AddMyFloat(Random.value);
    }
}
