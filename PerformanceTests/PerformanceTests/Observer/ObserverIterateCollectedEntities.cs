using Entitas;

public class ObserverIterateCollectedEntities : IPerformanceTest {
    const int n = 100000;
    GroupObserver _observer;

    public void Before() {
        var pool = new Pool(CP.NumComponents);
        var group = pool.GetGroup(Matcher.AllOf(new [] {CP.ComponentA}));
        _observer = group.CreateObserver();

        for (int i = 0; i < 1000; i++) {
            var e = pool.CreateEntity();
            e.AddComponent(CP.ComponentA, new ComponentA());
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            var entities = _observer.collectedEntities;
            for (var j = entities.GetEnumerator(); j.MoveNext();) {
                var e2 = j.Current;
            }
        }
    }
}

