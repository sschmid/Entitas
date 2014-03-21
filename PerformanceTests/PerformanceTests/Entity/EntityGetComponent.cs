using Entitas;

public class EntityGetComponent : IPerformanceTest {
    Entity _e;

    public void Before() {
        _e = new Entity();
        _e.AddComponent(new ComponentA());
        _e.AddComponent(new ComponentB());
        _e.AddComponent(new ComponentC());
    }

    public void Run() {
        var type = typeof(ComponentB);
        for (int i = 0; i < 100000; i++)
            _e.GetComponent(type);
    }
}

