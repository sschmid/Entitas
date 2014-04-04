using Entitas;

public class EntityRepositoryOnEntityReplaced : IPerformanceTest {
    EntityRepository _repo;
    Entity _e;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentA }));
        _e = _repo.CreateEntity();
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
    }
}

