using Entitas;

public class PoolOnEntityReplaced : IPerformanceTest {
    const int n = 100000;
    Pool _pool;
    Entity _e;

    public void Before() {
        _pool = Helper.CreatePool();
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _e = _pool.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}
