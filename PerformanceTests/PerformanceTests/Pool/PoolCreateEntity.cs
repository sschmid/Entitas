using Entitas;

public class PoolCreateEntity : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = Helper.CreatePool();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
    }
}
