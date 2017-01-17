using Entitas;

public class ContextHasEntity : IPerformanceTest {
    const int n = 100000;
    IContext<XXXEntity> _context;
    XXXEntity _e;

    public void Before() {
        _context = Helper.CreateContext();
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
