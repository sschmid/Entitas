using Entitas;

public class EntityHasComponent : IPerformanceTest {
    Entity _e;
    ComponentA _componentA;

    public void Before() {
        _e = new Entity();
        _componentA = new ComponentA();
        _e.AddComponent(_componentA);
        _e.AddComponent(new ComponentB());
        _e.AddComponent(new ComponentC());
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _e.HasComponent(_componentA);
    }
}

