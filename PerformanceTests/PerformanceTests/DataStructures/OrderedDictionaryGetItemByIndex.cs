using Entitas;
using System;
using System.Collections.Specialized;

public class OrderedDictionaryGetItemByIndex : IPerformanceTest {
    const int n = 100000;
    Random _random;
    OrderedDictionary _dict;

    public void Before() {
        _random = new Random();
        _dict = new OrderedDictionary();
        for (int i = 0; i < n; i++) {
            _dict.Add(i, new Entity(CP.NumComponents, null));
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var e = _dict[_random.Next(0, n)];
        }
    }
}

