using Entitas;
using System.Collections.Generic;

public class LinkedListAdd : IPerformanceTest {
    LinkedList<Entity> _ll;

    public void Before() {
        _ll = new LinkedList<Entity>();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _ll.AddLast(new Entity());
    }
}

