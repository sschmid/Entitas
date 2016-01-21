using Entitas;

public class EntityGetComponent : IPerformanceTest {
    const int n = 10000000;
    Entity _e;

    public void Before() {
        _e = new Entity(CP.NumComponents, null);
        _e.AddComponent(CP.ComponentA, new ComponentA());
        _e.AddComponent(CP.ComponentB, new ComponentB());
        _e.AddComponent(CP.ComponentC, new ComponentC());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.GetComponent(CP.ComponentA);
        }
    }
}

