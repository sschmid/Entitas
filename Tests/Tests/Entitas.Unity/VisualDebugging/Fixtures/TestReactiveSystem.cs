using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem {

    public TestReactiveSystem(Contexts contexts) : base(contexts.test) { }

    protected override Collector GetTrigger(Context context) {
        return context.CreateCollector(Matcher.AllOf(0));
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
    }
}
