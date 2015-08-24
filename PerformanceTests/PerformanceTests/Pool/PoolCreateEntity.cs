using Entitas;

public class PoolCreateEntity : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
    }
}

