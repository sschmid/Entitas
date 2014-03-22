using System.Collections.Generic;
using Entitas;

public class ListRemove : IPerformanceTest {
    List<Entity> _l;
    Entity[] _lookup;

    public void Before() {
        _l = new List<Entity>();
        _lookup = new Entity[100000];
        for (int i = 0; i < 100000; i++) {
            var e = new Entity();
            _l.Add(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            _l.Remove(_lookup[i]);
        }
    }
}

