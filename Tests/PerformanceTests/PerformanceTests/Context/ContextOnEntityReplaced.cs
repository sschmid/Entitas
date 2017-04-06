using Entitas;
using Entitas.Core;

public class ContextOnEntityReplaced : IPerformanceTest {

    const int n = 100000;
    IContext<Entity> _context;
    IEntity _e;

    public void Before() {
        _context = Helper.CreateContext();
        _context.GetGroup(Matcher<Entity>.AllOf(new [] { CP.ComponentA }));
        _e = _context.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}
