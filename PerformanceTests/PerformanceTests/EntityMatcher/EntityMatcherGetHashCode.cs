using Entitas;

public class EntityMatcherGetHashCode : IPerformanceTest {
    IEntityMatcher _m;

    public void Before() {
        _m = EntityMatcher.AllOf(new [] {
            CP.ComponentA,
            CP.ComponentB,
            CP.ComponentC
        });
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _m.GetHashCode();
    }
}

