using Entitas;

public class EntityIndexGetEntity : IPerformanceTest {

    const int n = 1000000;

    IContext<XXXEntity> _context;

    PrimaryEntityIndex<XXXEntity, string> _index;

    public void Before() {
        _context = Helper.CreateContext();
        _index = new PrimaryEntityIndex<XXXEntity, string>(_context.GetGroup(Matcher<XXXEntity>.AllOf(CP.ComponentA)), (e, c) => ((NameComponent)c).name);

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
