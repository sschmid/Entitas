using System.Collections.Generic;
using Entitas;

public class SomeInitializeReactiveSystem : ReactiveSystem, IInitializeSystem {

    public SomeInitializeReactiveSystem(Contexts contexts) : base(
        contexts.visualDebugging.CreateCollector(Matcher.AllOf(0))
    ) { }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public void Initialize() {
    }

    public override void Execute(List<Entity> entities) {
    }
}
