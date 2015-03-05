using System.Collections.Generic;

public class IterateHashSet : IPerformanceTest {
    const int n = 10000;

    HashSet<int> _set;

    public void Before() {
        _set = new HashSet<int>();
        for (int i = 0; i < 10000; i++) {
            _set.Add(i);
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            foreach (var item in _set) {
                var j = item;
            }
        }
    }
}

