using System.Collections.Generic;

public class IterateHashetToArray : IPerformanceTest {
    const int n = 10000;

    HashSet<int> _set;

    public void Before() {
        _set = new HashSet<int>();
        for (int i = 0; i < 10000; i++) {
            _set.Add(i);
        }
    }

    #pragma warning disable
    public void Run() {
        for (int i = 0; i < n; i++) {
            var array = new int[_set.Count];
            _set.CopyTo(array);
            for (int j = 0, arrayLength = array.Length; j < arrayLength; j++) {
                var k = array[j];
            }
        }
    }
}


