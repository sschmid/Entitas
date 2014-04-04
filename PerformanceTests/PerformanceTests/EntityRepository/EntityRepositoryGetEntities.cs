using Entitas;

public class EntityRepositoryGetEntities : IPerformanceTest {
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        for (int i = 0; i < 100000; i++)
            _repo.CreateEntity();
    }

    public void Run() {
        _repo.GetEntities();
    }
}

