using Entitas;

public class ContextHasEntity : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = new Context(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _context.HasEntity(new Entity(CP.NumComponents));
        }
    }
}

