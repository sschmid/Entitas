using Entitas;
using Entitas.Serialization.Blueprints;

public class PoolCreateBlueprint : IPerformanceTest {
    const int n = 100000;
    Pool _pool;
    Blueprint _blueprint;

    public void Before() {
        _pool = Helper.CreatePool();

        var e = _pool.CreateEntity();
        var component = new NameAgeComponent();
        component.name = "Max";
        component.age = 42;
        e.AddComponent(CP.ComponentA, component);

        _blueprint = new Blueprint(string.Empty, e);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity().ApplyBlueprint(_blueprint);
        }
    }
}

public class NameAgeComponent : IComponent {
    public string name;
    public int age;
}
