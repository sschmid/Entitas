using System.Collections.Generic;
using Entitas;
using Entitas.Api;

public class TestReactiveSystem : ReactiveSystem<VisualDebuggingEntity> {

    public TestReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector<VisualDebuggingEntity> GetTrigger(IContext<VisualDebuggingEntity> context) {
        return context.CreateCollector(VisualDebuggingMatcher.Test);
    }

    protected override bool Filter(VisualDebuggingEntity entity) {
        return true;
    }

    protected override void Execute(List<VisualDebuggingEntity> entities) {
    }
}
