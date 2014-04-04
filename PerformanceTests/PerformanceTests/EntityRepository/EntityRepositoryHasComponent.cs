using Entitas;

public class EntityRepositoryHasComponent : IPerformanceTest {
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        for (int i = 0; i < 100000; i++)
            _repo.CreateEntity();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _repo.HasEntity(new Entity(CP.NumComponents));
    }
}

