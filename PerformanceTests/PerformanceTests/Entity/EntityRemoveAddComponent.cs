using Entitas;

public class EntityRemoveAddComponent : IPerformanceTest {
    const int n = 1000000;
    Pool _pool;
    Entity _e;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentB }));
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentC }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentC
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentB,
            CP.ComponentC
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        }));
        _e = _pool.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.RemoveComponent(CP.ComponentA);
            _e.AddComponent(CP.ComponentA, new ComponentA());
        }    
    }
}

