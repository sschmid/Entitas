using System.Collections.Generic;
using Entitas;
using System;

public class DictionaryGetItem : IPerformanceTest {
    const int n = 1000000;
    const int elements = 5000;
    Random _random;
    Dictionary<int, Entity> _dict;

    public void Before() {
        _random = new Random();
        _dict = new Dictionary<int, Entity>();
        for (int i = 0; i < elements; i++) {
            _dict.Add(i, new Entity(CP.NumComponents, null));
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var e = _dict[_random.Next(0, elements)];
        }
    }
}
