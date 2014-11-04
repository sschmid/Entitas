using Entitas;

public class EntityReplaceComponent : IPerformanceTest {
    const int n = 1000000;
    EntityRepository _repo;
    Entity _e;

    public void Before() {
        _repo = new EntityRepository(CP.NumComponents);
        _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentA }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentB }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentC }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentC
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            CP.ComponentB,
            CP.ComponentC
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        }));
        _e = new Entity(CP.NumComponents);
        _e.AddComponent(CP.ComponentA, new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.ReplaceComponent(CP.ComponentA, new ComponentA());
        }
    }
}

