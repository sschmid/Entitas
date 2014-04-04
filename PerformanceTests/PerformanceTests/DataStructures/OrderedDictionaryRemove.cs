using Entitas;
using System.Collections.Specialized;

public class OrderedDictionaryRemove : IPerformanceTest {
    OrderedDictionary _d;
    Entity[] _lookup;

    public void Before() {
        _d = new OrderedDictionary();
        _lookup = new Entity[100000];
        for (int i = 0; i < 100000; i++) {
            var e = new Entity(CP.NumComponents);
            _d.Add(i, e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < 100000; i++) {
            _d.Remove(_lookup[i]);
        }
    }
}

