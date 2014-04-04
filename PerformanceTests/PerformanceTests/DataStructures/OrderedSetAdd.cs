using Entitas;
using ToolKit;

public class OrderedSetAdd : IPerformanceTest {
    OrderedSet<Entity> _s;

    public void Before() {
        _s = new OrderedSet<Entity>();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _s.Add(new Entity(CP.NumComponents));
    }
}

