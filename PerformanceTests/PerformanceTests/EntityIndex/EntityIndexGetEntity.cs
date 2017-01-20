using Entitas;
using Entitas.Api;

public class EntityIndexGetEntity : IPerformanceTest {

    const int n = 1000000;

    IContext<Entity> _context;

    PrimaryEntityIndex<Entity, string> _index;

    public void Before() {
        _context = Helper.CreateContext();
        _index = new PrimaryEntityIndex<Entity, string>(_context.GetGroup(Matcher<Entity>.AllOf(CP.ComponentA)), (e, c) => ((NameComponent)c).name);

        for (int i = 0; i < 10; i++) {
            var nameComponent = new NameComponent();
            nameComponent.name = i.ToString();
            _context.CreateEntity().AddComponent(CP.ComponentA, nameComponent);
        }
    }

    public void Run() {
        var name = 9.ToString();
        for (int i = 0; i < n; i++) {
            _index.GetEntity(name);
        }
    }
}
