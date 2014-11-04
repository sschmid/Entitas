using Entitas;

public class EntityRepositoryHasComponent : IPerformanceTest {
    const int n = 100000;
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        for (int i = 0; i < n; i++) {
            _repo.CreateEntity();
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _repo.HasEntity(new Entity(CP.NumComponents));
        }
    }
}

