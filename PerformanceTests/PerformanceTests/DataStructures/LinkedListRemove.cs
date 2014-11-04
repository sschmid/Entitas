using System.Collections.Generic;
using Entitas;

public class LinkedListRemove : IPerformanceTest {
    const int n = 100000;
    LinkedList<Entity> _ll;
    Entity[] _lookup;

    public void Before() {
        _ll = new LinkedList<Entity>();
        _lookup = new Entity[n];
        for (int i = 0; i < n; i++) {
            var e = new Entity(CP.NumComponents);
            _ll.AddLast(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _ll.Remove(_lookup[i]);
        }
    }
}

