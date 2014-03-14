using Entitas;

public class EntityRepositoryDestroyAllEntites : IPerformanceTest {
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository();
        for (int i = 0; i < 100000; i++)
            _repo.CreateEntity();
    }

    public void Run() {
        _repo.DestroyAllEntities();
    }
}

