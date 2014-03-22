using Entitas;
using System;
using System.Collections.Specialized;

public class OrderedDictionaryGetItemByKey : IPerformanceTest {
    Random _r;
    OrderedDictionary _d;

    public void Before() {
        _r = new Random();
        _d = new OrderedDictionary();
        for (int i = 0; i < 100000; i++) {
            _d.Add(i, new Entity());
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            var e = _d[_r.Next(0, 100000)];
        }
    }
}

