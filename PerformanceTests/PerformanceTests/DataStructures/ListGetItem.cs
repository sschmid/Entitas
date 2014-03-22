using System.Collections.Generic;
using Entitas;
using System;

public class ListGetItem : IPerformanceTest {
    Random _r;
    List<Entity> _l;

    public void Before() {
        _r = new Random();
        _l = new List<Entity>();
        for (int i = 0; i < 100000; i++) {
            _l.Add(new Entity());
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            var e = _l[_r.Next(0, 100000)];
        }
    }
}

