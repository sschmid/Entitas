using Entitas;

public class EntityRepositoryCreateEntity : IPerformanceTest {
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _repo.CreateEntity();
    }
}

