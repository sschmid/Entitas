using System.Collections.Generic;
using Entitas;
using System;

public class DictionaryGetItem : IPerformanceTest {
    Random _r;
    Dictionary<int, Entity> _d;

    public void Before() {
        _r = new Random();
        _d = new Dictionary<int, Entity>();
        for (int i = 0; i < 100000; i++) {
            _d.Add(i, new Entity(CP.NumComponents));
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            var e = _d[_r.Next(0, 100000)];
        }
    }
}

