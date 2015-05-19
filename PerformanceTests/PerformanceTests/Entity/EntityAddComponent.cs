using Entitas;

public class EntityAddComponent : IPerformanceTest {
    const int n = 10000000;
    Entity _e;
    ComponentA _componentA;

    public void Before() {
        _e = new Entity(CP.NumComponents);
        _componentA = new ComponentA();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.AddComponent(CP.ComponentA, _componentA);
            _e.RemoveComponent(CP.ComponentA);
        }
    }
}

