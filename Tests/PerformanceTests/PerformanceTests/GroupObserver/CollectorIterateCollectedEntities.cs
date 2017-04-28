using Entitas;

public class CollectorIterateCollectedEntities : IPerformanceTest {

    const int n = 100000;
    ICollector<Entity> _collector;

    public void Before() {
        var context = Helper.CreateContext();
        var group = context.GetGroup(Matcher<Entity>.AllOf(new [] { CP.ComponentA }));
        _collector = group.CreateCollector();

        for (int i = 0; i < 1000; i++) {
            var e = context.CreateEntity();
            e.AddComponent(CP.ComponentA, new ComponentA());
        }
    }

    public void Run() {
        var entities = _collector.collectedEntities;
        for (int i = 0; i < n; i++) {
            for (var j = entities.GetEnumerator(); j.MoveNext();) {
                var e2 = j.Current;
            }
        }
    }
}
