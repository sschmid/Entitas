using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem {

    readonly Pool _pool;

    public ProcessRandomValueSystem(Pools pools) : base(
        pools.visualDebugging.CreateCollector(VisualDebuggingMatcher.MyFloat))
    {
        _pool = pools.visualDebugging;
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
        foreach(var e in entities) {
            _pool.DestroyEntity(e);
        }
    }
}
