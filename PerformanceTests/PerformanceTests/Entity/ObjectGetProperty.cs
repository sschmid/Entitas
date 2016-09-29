using Entitas;

public class ObjectGetProperty : IPerformanceTest {
    const int n = 10000000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(1);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var c = _pool.totalComponents;
        }
    }
}
