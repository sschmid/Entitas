using Entitas;
using System.Collections.Generic;

public class EntityAddComponent : IPerformanceTest {
    const int n = 10000000;
    Entity _e;
    ComponentA _componentA;

    public void Before() {
        var pool = Helper.CreatePool();
        _e = pool.CreateEntity();
        _componentA = new ComponentA();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var componentPool = _e.GetComponentPool(CP.ComponentA);
            var component = componentPool.Count > 0 ? componentPool.Pop() : _componentA;
            _e.AddComponent(CP.ComponentA, component);
            _e.RemoveComponent(CP.ComponentA);
        }
    }
}

