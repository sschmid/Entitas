using System.Collections.Generic;
using Entitas;

public class ListAdd : IPerformanceTest {
    List<Entity> _l;

    public void Before() {
        _l = new List<Entity>();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _l.Add(new Entity(CP.NumComponents));
    }
}

