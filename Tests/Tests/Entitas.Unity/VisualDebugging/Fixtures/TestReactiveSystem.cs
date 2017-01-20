using System.Collections.Generic;
using Entitas;
using Entitas.Api;

public class TestReactiveSystem : ReactiveSystem<TestEntity> {

    public TestReactiveSystem(Contexts contexts) : base(contexts.test) { }

    protected override Collector<TestEntity> GetTrigger(IContext<TestEntity> context) {
        return context.CreateCollector(Matcher<TestEntity>.AllOf(0));
    }

    protected override bool Filter(TestEntity entity) {
        return true;
    }

    protected override void Execute(List<TestEntity> entities) {
    }
}
