using Entitas;

public class EntityIndexGetEntity : IPerformanceTest {

    const int n = 1000000;

    Context _context;

    PrimaryEntityIndex<string> _index;

    public void Before() {
        _context = Helper.CreatePool();
        _index = new PrimaryEntityIndex<string>(_context.GetGroup(Matcher.AllOf(CP.ComponentA)), (e, c) => ((NameComponent)c).name);

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
