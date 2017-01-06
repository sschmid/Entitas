using Entitas;

public class PoolHasEntity : IPerformanceTest {
    const int n = 100000;
    Context _pool;
    Entity _e;

    public void Before() {
        _pool = Helper.CreatePool();
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
        _e = _pool.CreateEntity();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _pool.HasEntity(_e);
        }
    }
}
