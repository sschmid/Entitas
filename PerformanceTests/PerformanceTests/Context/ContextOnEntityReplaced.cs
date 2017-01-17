using Entitas;

public class ContextOnEntityReplaced : IPerformanceTest {
    const int n = 100000;
    IContext<XXXEntity> _context;
    IEntity _e;

    public void Before() {
        _context = Helper.CreateContext();
        _context.GetGroup(Matcher<XXXEntity>.AllOf(new [] { CP.ComponentA }));
        _e = _context.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}
