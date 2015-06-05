using Entitas;
using System;

public class ArrayGetItem : IPerformanceTest {
    const int n = 1000000;
    const int elements = 5000;
    Random random;
    Entity[] _array;

    public void Before() {
        random = new Random();
        _array = new Entity[elements];
        for (int i = 0; i < elements; i++) {
            _array[i] = new Entity(CP.NumComponents);
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var e = _array[random.Next(0, elements)];
        }
    }
}


