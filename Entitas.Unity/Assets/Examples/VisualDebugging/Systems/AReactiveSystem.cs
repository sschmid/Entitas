using System.Collections.Generic;
using System.Threading;
using Entitas;
using Entitas.Api;

public class AReactiveSystem : ReactiveSystem<VisualDebuggingEntity> {

    public AReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector<VisualDebuggingEntity> GetTrigger(IContext<VisualDebuggingEntity> context) {
        return context.CreateCollector(VisualDebuggingMatcher.MyString);
    }

    protected override bool Filter(VisualDebuggingEntity entity) {
        return true;
    }

    protected override void Execute(List<VisualDebuggingEntity> entities) {
        Thread.Sleep(2);
    }
}
