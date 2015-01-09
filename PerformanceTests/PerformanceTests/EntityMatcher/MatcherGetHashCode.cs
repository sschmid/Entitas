using Entitas;

public class MatcherGetHashCode : IPerformanceTest {
    const int n = 10000000;
    IMatcher _m;

    public void Before() {
        _m = Matcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        });
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _m.GetHashCode();
        }
    }
}

