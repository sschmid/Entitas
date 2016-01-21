using Entitas;
using System.Collections.Specialized;

public class OrderedDictionaryAdd : IPerformanceTest {
    const int n = 100000;
    OrderedDictionary _dict;

    public void Before() {
        _dict = new OrderedDictionary();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _dict.Add(i, new Entity(CP.NumComponents, null));
        }
    }
}

