using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem {

    public TestReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector GetTrigger(Context context) {
        return context.CreateCollector(VisualDebuggingMatcher.Test);
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
