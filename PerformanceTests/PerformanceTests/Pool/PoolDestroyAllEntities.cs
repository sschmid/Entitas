using Entitas;

public class PoolDestroyAllEntities : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = Helper.CreatePool();
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
    }

    public void Run() {
        _pool.DestroyAllEntities();
    }
}
