using Entitas;

public class ContextGetGroup : IPerformanceTest {
    const int n = 100000;
    IContext<XXXEntity> _context;

    public void Before() {
        _context = Helper.CreateContext();
    }

    public void Run() {
        var m = Matcher<XXXEntity>.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _context.GetGroup(m);
        }
    }
}
