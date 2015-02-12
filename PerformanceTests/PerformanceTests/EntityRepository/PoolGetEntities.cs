using Entitas;

public class PoolGetEntities : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
    }

    public void Run() {
        _pool.GetEntities();
    }
}

