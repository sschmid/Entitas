using Entitas;

public class EntityRemoveAddComponent : IPerformanceTest {
    const int n = 1000000;
    Context _pool;
    Entity _e;
    ComponentA _componentA;

    public void Before() {
        _pool = Helper.CreatePool();
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentB }));
        _pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentC }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentC
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentB,
            CP.ComponentC
        }));
        _pool.GetGroup(Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        }));
        _e = _pool.CreateEntity();
        _componentA = new ComponentA();
        _e.AddComponent(CP.ComponentA, _componentA);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _e.RemoveComponent(CP.ComponentA);
            _e.AddComponent(CP.ComponentA, _e.CreateComponent<ComponentA>(CP.ComponentA));
        }    
    }
}
