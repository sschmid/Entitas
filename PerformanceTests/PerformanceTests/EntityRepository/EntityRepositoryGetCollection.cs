using Entitas;

public class EntityRepositoryGetCollection : IPerformanceTest {
    const int n = 100000;
    EntityRepository _repo;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
    }

    public void Run() {
        var m = EntityMatcher.AllOf(new [] { CP.ComponentA });
        for (int i = 0; i < n; i++) {
            _repo.GetCollection(m);
        }
    }
}

