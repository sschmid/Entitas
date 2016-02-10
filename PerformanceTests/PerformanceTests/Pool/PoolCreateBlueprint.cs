using Entitas;
using System.Collections.Generic;

public class PoolCreateBlueprint : IPerformanceTest {
    const int n = 100000;
    Pool _pool;
    Blueprint _blueprint;

    public void Before() {
        _pool = Helper.CreatePool();
        var fields = new Dictionary<string, object> {
            { "name", "Max" },
            { "age", 42 }
        };
        var componentBlueprint = new ComponentBlueprint(0, typeof(NameAgeComponent).FullName, fields);
        var components = new [] {
            componentBlueprint
        };

        _blueprint = new Blueprint(string.Empty, components);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _pool.CreateEntity(_blueprint);
        }
    }
}

public class NameAgeComponent : IComponent {
    public string name;
    public int age;
}
