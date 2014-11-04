using Entitas;

public class EntityRepositoryOnEntityReplaced : IPerformanceTest {
    const int n = 100000;
    EntityRepository _repo;
    Entity _e;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentA }));
        _e = _repo.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}

