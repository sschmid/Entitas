using Entitas;

public class PoolDestroyEntity : IPerformanceTest {
    const int n = 100000;
    Context _context;

    public void Before() {
        _context = Helper.CreatePool();
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
