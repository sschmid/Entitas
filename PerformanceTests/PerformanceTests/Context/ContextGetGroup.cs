using Entitas;
using Entitas.Api;

public class ContextGetGroup : IPerformanceTest {
    const int n = 100000;
    IContext<Entity> _context;

    public void Before() {
        _context = Helper.CreateContext();
    }

    public void Run() {
        var m = Matcher<Entity>.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _context.GetGroup(m);
        }
    }
}
