using Entitas;
using Entitas.Serialization.Blueprints;

public class ContextCreateBlueprint : IPerformanceTest {
    const int n = 100000;
    Context _context;
    Blueprint _blueprint;

    public void Before() {
        _context = Helper.CreateContext();

        var e = _context.CreateEntity();
        var component = new NameAgeComponent();
        component.name = "Max";
        component.age = 42;
        e.AddComponent(CP.ComponentA, component);

        _blueprint = new Blueprint(string.Empty, string.Empty, e);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {

            // TODO Once this method moved to IEntity
            // uncomment this code

            //_context.CreateEntity().ApplyBlueprint(_blueprint);
        }
    }
}

public class NameAgeComponent : IComponent {
    public string name;
    public int age;
}
