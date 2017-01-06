using Entitas;

public class PoolHasEntity : IPerformanceTest {
    const int n = 100000;
    Context _context;
    Entity _e;

    public void Before() {
        _context = Helper.CreatePool();
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
        _e = _context.CreateEntity();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _context.HasEntity(_e);
        }
    }
}
