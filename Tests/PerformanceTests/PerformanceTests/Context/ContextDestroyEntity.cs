using Entitas;

public class ContextDestroyEntity : IPerformanceTest {

    const int n = 100000;
    IContext<Entity> _context;

    public void Before() {
        _context = Helper.CreateContext();
        for (int i = 0; i < n; i++) {
            _context.CreateEntity();
        }
    }

    public void Run() {
        var entities = _context.GetEntities();
        for (int i = 0; i < entities.Length; i++) {
            _context.DestroyEntity(entities[i]);
        }
    }
}
