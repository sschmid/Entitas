using Entitas;
using System.Collections.Specialized;

public class OrderedDictionaryRemove : IPerformanceTest {
    const int n = 100000;
    OrderedDictionary _dict;
    Entity[] _lookup;

    public void Before() {
        _dict = new OrderedDictionary();
        _lookup = new Entity[n];
        for (int i = 0; i < n; i++) {
            var e = new Entity(CP.NumComponents, null);
            _dict.Add(i, e);
            _lookup[i] = e;
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _dict.Remove(_lookup[i]);
        }
    }
}
