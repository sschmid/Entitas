using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : ReactiveSystem {

    public SomeReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector GetTrigger(Context context) {
        return context.CreateCollector(Matcher.AllOf(0));
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
