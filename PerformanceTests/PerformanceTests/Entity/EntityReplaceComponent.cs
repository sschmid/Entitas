using Entitas;

public class EntityReplaceComponent : IPerformanceTest {
    const int n = 1000000;
    Context _context;
    Entity _e;

    public void Before() {
        _context = new Context(CP.NumComponents);
        _context.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _context.GetGroup(Matcher.AllOf(new [] { CP.ComponentB }));
        _context.GetGroup(Matcher.AllOf(new [] { CP.ComponentC }));
        _context.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        }));
        _context.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentC
        }));
        _context.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentB,
            CP.ComponentC
        }));
        _context.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        }));
        _e = new Entity(CP.NumComponents);
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}

