using Entitas;

public class EntityReplaceComponet : IPerformanceTest {
    EntityRepository _repo;
    Entity _e;

    public void Before() {
        _repo = new EntityRepository();
        _repo.GetCollection(EntityMatcher.AllOf(new [] { typeof(ComponentA) }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] { typeof(ComponentB) }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] { typeof(ComponentC) }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB)
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentC)
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            typeof(ComponentB),
            typeof(ComponentC)
        }));
        _repo.GetCollection(EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB),
            typeof(ComponentC)
        }));
        _e = _repo.CreateEntity();
        _e.AddComponent(new ComponentA());
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _e.ReplaceComponent(new ComponentA());
    }
}

