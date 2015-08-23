using Entitas;

public class PoolDestroyEntity : IPerformanceTest {
    const int n = 100000;
    Pool _pool;

    public void Before() {
        _pool = new Pool(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity();
        }
    }

    public void Run() {
        var entities = _pool.GetEntities();
        for (int i = 0; i < entities.Length; i++) {
            _pool.DestroyEntity(entities[i]);
        }
    }
}

