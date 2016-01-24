using System.Collections.Generic;
using Entitas;

public class ListRemove : IPerformanceTest {
    const int n = 100000;
    List<Entity> _l;
    Entity[] _lookup;

    public void Before() {
        _l = new List<Entity>();
        _lookup = new Entity[n];
        for (int i = 0; i < n; i++) {
            var e = new Entity(CP.NumComponents, null);
            _l.Add(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _l.Remove(_lookup[i]);
        }
    }
}

