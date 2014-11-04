using Entitas;
using ToolKit;

public class OrderedSetAdd : IPerformanceTest {
    const int n = 100000;
    LinkedListSet<Entity> _s;

    public void Before() {
        _s = new LinkedListSet<Entity>();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _s.Add(new Entity(CP.NumComponents));
        }
    }
}

