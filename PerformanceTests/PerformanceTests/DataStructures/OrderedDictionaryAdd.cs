using Entitas;
using System.Collections.Specialized;

public class OrderedDictionaryAdd : IPerformanceTest {
    OrderedDictionary _d;

    public void Before() {
        _d = new OrderedDictionary();
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _d.Add(i, new Entity());
    }
}

