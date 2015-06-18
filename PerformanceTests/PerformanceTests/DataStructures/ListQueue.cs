using System.Collections.Generic;

public class ListQueue : IPerformanceTest {
    const int n = 1000000;
    const int elements = 60;
    List<float> _list;

    public void Before() {
        _list = new List<float>(elements);
        for (int i = 0; i < elements; i++) {
            _list.Add(1f);
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _list.RemoveAt(0);
            _list.Add(1f);
        }
    }
}


