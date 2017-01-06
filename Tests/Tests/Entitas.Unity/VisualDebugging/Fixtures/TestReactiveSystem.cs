using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem {

    public TestReactiveSystem(Contexts pools) : base(
        pools.test.CreateCollector(Matcher.AllOf(0))
    ) { }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
