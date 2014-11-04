using Entitas;
using ToolKit;

public class OrderedSetRemove : IPerformanceTest {
    const int n = 100000;
    LinkedListSet<Entity> _s;
    Entity[] _lookup;

    public void Before() {
        _s = new LinkedListSet<Entity>();
        _lookup = new Entity[n];
        for (int i = 0; i < n; i++) {
            var e = new Entity(CP.NumComponents);
            _s.Add(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _s.Remove(_lookup[i]);
        }
    }
}

