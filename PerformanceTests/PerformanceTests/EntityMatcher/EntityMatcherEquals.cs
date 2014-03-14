using Entitas;

public class EntityMatcherEquals : IPerformanceTest {
    IEntityMatcher _m1;
    IEntityMatcher _m2;

    public void Before() {
        _m1 = EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB),
            typeof(ComponentC)
        });
        _m2 = EntityMatcher.AllOf(new [] {
            typeof(ComponentA),
            typeof(ComponentB),
            typeof(ComponentC)
        });
    }

    public void Run() {
        for (int i = 0; i < 100000; i++)
            _m1.Equals(_m2);
    }
}

