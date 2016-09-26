using Entitas;

public class EntityCollectorIterateCollectedEntities : IPerformanceTest {

    const int n = 100000;
    EntityCollector _collector;

    public void Before() {
        var pool = Helper.CreatePool();
        var group = pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _collector = group.CreateCollector();

        for(int i = 0; i < 1000; i++) {
            var e = pool.CreateEntity();
            e.AddComponent(CP.ComponentA, new ComponentA());
        }
    }

    public void Run() {
        var entities = _collector.collectedEntities;
        for(int i = 0; i < n; i++) {
            for(var j = entities.GetEnumerator(); j.MoveNext();) {
                var e2 = j.Current;
            }
        }
    }
}

