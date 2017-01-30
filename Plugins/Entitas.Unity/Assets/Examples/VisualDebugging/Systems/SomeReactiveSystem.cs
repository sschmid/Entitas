using System.Collections.Generic;
using Entitas;
using Entitas.Api;

public class SomeReactiveSystem : ReactiveSystem<VisualDebuggingEntity> {

    public SomeReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector<VisualDebuggingEntity> GetTrigger(IContext<VisualDebuggingEntity> context) {
        return context.CreateCollector(Matcher<VisualDebuggingEntity>.AllOf(0));
    }

    protected override bool Filter(VisualDebuggingEntity entity) {
        return true;
    }

    protected override void Execute(List<VisualDebuggingEntity> entities) {
    }
}
