using System.Collections.Generic;

public class HashSetContainsAdd : IPerformanceTest {
    const int n = 1000000;
    const int elements = 5000;
    HashSet<object> _set;

    public void Before() {
        _set = new HashSet<object>();
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var o = new object(); 
//            if(!_set.Contains(o)) {
//                _set.Add(o);
//            }

            var added = _set.Add(o);
            if(!added) {
                
            }
        }
    }
}
