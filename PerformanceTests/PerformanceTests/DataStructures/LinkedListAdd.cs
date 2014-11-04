using Entitas;
using System.Collections.Generic;

public class LinkedListAdd : IPerformanceTest {
    const int n = 100000;
    LinkedList<Entity> _ll;

    public void Before() {
        _ll = new LinkedList<Entity>();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _ll.AddLast(new Entity(CP.NumComponents));
        }
    }
}

