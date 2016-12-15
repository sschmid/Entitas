using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : ReactiveSystem {

    public SomeReactiveSystem(Pools pools) : base(
        pools.visualDebugging.CreateCollector(Matcher.AllOf(0))
    ) { }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
