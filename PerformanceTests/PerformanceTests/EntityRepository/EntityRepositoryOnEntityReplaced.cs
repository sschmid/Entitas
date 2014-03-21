using Entitas;

public class EntityRepositoryOnEntityReplaced : IPerformanceTest {
    EntityRepository _repo;
    Entity _e;

    public void Before() {
        _repo = new EntityRepository();
        _repo.GetCollection<ComponentA>();
        _e = _repo.CreateEntity();
        _e.AddComponent(new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _e.ReplaceComponent(new ComponentA());
    }
}

