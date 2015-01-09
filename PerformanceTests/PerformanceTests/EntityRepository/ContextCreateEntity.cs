using Entitas;

public class ContextCreateEntity : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = new Context(CP.NumComponents);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
    }
}

