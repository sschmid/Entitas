using Entitas;
using UnityEngine;

public class RandomValueSystem : IExecuteSystem, ISetPool {
    Pool _pool;

    public void SetPool(Pool pool) {
        _pool = pool;
    }

    public void Execute() {
        _pool.CreateEntity().AddMyFloat(Random.value);
    }
}

