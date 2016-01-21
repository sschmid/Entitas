using Entitas;

public class PoolHasEntity : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _pool.HasEntity(new Entity(CP.NumComponents, null));
        }
    }
}

