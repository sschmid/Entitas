using System.Collections.Generic;
using Entitas;

public class LinkedListRemove : IPerformanceTest {
    LinkedList<Entity> _ll;
    Entity[] _lookup;

    public void Before() {
        _ll = new LinkedList<Entity>();
        _lookup = new Entity[100000];
        for (int i = 0; i < 100000; i++) {
            var e = new Entity(CP.NumComponents);
            _ll.AddLast(e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            _ll.Remove(_lookup[i]);
        }
    }
}

