using Entitas;

public class PoolGetGroup : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
    }

    public void Run() {
        var m = Matcher.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _pool.GetGroup(m);
        }
    }
}

