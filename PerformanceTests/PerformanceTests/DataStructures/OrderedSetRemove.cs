using Entitas;
using ToolKit;

public class OrderedSetRemove : IPerformanceTest {
    OrderedSet<Entity> _s;
    Entity[] _lookup;

    public void Before() {
        _s = new OrderedSet<Entity>();
        _lookup = new Entity[100000];
        for (int i = 0; i < 100000; i++) {
            var e = new Entity();
            _s.Add(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            _s.Remove(_lookup[i]);
        }
    }
}

