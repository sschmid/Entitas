using Entitas;

public class EntityGetComponents : IPerformanceTest {
    Entity _e;

    public void Before() {
        _e = new Entity();
        _e.AddComponent(new ComponentA());
        _e.AddComponent(new ComponentB());
        _e.AddComponent(new ComponentC());
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _e.GetComponents();
    }
}

