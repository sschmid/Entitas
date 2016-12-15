using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem {

    public TestReactiveSystem(Pools pools) : base(
        pools.visualDebugging.CreateCollector(VisualDebuggingMatcher.Test)
    ) { }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
