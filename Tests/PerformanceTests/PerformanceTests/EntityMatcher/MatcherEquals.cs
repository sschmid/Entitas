using Entitas;

public class MatcherEquals : IPerformanceTest {

    const int n = 10000000;
    IMatcher<Entity> _m1;
    IMatcher<Entity> _m2;

    public void Before() {
        _m1 = Matcher<Entity>.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        });
        _m2 = Matcher<Entity>.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        });
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _m1.Equals(_m2);
        }
    }
}
