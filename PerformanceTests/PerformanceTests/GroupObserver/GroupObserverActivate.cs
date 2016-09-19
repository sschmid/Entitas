using Entitas;

public class GroupObserverActivate : IPerformanceTest {

    const int n = 10000;
    GroupObserver _observer;

    public void Before() {
        var pool = Helper.CreatePool();
        var group = pool.GetGroup(Matcher.AllOf(new [] { CP.ComponentA }));
        _observer = group.CreateObserver();
    }

    public void Run() {
        for(int i = 0; i < n; i++) {
            _observer.Activate();
        }
    }
}

