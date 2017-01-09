using Entitas;

public class ContextGetGroup : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = Helper.CreateContext();
    }

    public void Run() {
        var m = Matcher.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _context.GetGroup(m);
        }
    }
}
