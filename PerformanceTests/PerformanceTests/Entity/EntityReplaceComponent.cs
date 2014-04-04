using Entitas;

public class EntityReplaceComponent : IPerformanceTest {
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
        var componentA = new ComponentA();
        var type = CP.ComponentA;
        for (int i = 0; i < 100000; i++)
            _e.ReplaceComponent(type, componentA);
    }
}

