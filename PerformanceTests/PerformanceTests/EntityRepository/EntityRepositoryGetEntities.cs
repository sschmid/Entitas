using Entitas;

public class EntityRepositoryGetEntities : IPerformanceTest {
    const int n = 100000;
    EntityRepository _repo;


    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _repo.CreateEntity();
        }
    }

    public void Run() {
        _repo.GetEntities();
    }
}

