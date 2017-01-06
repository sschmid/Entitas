using Entitas;

public class PoolGetGroup : IPerformanceTest {
    const int n = 100000;
    Context _pool;

    public void Before() {
        _pool = Helper.CreatePool();
    }

    public void Run() {
        var m = Matcher.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _pool.GetGroup(m);
        }
    }
}
