using Entitas;

public class GroupObserverIterateCollectedEntities : IPerformanceTest {

    const int n = 100000;
    EntityCollector _observer;

    public void Before() {
        var pool = Helper.CreatePool();
        var group = pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _observer = group.CreateObserver();

        for(int i = 0; i < 1000; i++) {
            var e = pool.CreateEntity();
            e.AddComponent(CP.ComponentA, new ComponentA());
        }
    }

    public void Run() {
        var entities = _observer.collectedEntities;
        for(int i = 0; i < n; i++) {
            for(var j = entities.GetEnumerator(); j.MoveNext();) {
                var e2 = j.Current;
            }
        }
    }
}

