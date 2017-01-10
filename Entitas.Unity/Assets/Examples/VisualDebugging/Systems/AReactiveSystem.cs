using System.Collections.Generic;
using System.Threading;
using Entitas;

public class AReactiveSystem : ReactiveSystem {

    public AReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector GetTrigger(Context context) {
        return context.CreateCollector(VisualDebuggingMatcher.MyString);
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
        Thread.Sleep(2);
    }
}
