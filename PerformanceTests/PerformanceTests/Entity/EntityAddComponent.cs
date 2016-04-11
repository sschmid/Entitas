using Entitas;

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
            _e.AddComponent(CP.ComponentA, _e.CreateComponent<ComponentA>(CP.ComponentA));
            _e.RemoveComponent(CP.ComponentA);
        }
    }
}

