using Entitas;

public class EntityMatcherEquals : IPerformanceTest {
    const int n = 10000000;
    IEntityMatcher _m1;
    IEntityMatcher _m2;

    public void Before() {
        _m1 = EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        });
        _m2 = EntityMatcher.AllOf(new [] {
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

