using Entitas;

public class PoolOnEntityReplaced : IPerformanceTest {
    const int n = 100000;
    Context _context;
    Entity _e;

    public void Before() {
        _context = Helper.CreateContext();
        _context.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _e = _context.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}
