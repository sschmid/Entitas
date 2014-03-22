using Entitas;
using System;

public class ArrayGetItem : IPerformanceTest {
    Random _r;
    Entity[] _a;

    public void Before() {
        _r = new Random();
        _a = new Entity[100000];
        for (int i = 0; i < 100000; i++) {
            _a[i] = new Entity();
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            var e = _a[_r.Next(0, 100000)];
        }
    }
}


