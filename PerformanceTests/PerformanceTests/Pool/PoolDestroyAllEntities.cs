using Entitas;

public class PoolDestroyAllEntities : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = Helper.CreatePool();
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
    }

    public void Run() {
        _context.DestroyAllEntities();
    }
}
