using Entitas;

public class EntityHasComponent : IPerformanceTest {
    const int n = 1000000;
    Entity _e;

    public void Before() {
        _e = new Entity(CP.NumComponents);
        _e.AddComponent(CP.ComponentA, new ComponentA());
        _e.AddComponent(CP.ComponentB, new ComponentB());
        _e.AddComponent(CP.ComponentC, new ComponentC());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.HasComponent(CP.ComponentA);
        }
    }
}

