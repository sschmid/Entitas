using System.Collections.Generic;
using Entitas;

public class SomeInitializeReactiveSystem : ReactiveSystem<VisualDebuggingEntity>, IInitializeSystem {

    public SomeInitializeReactiveSystem(Contexts contexts) : base(contexts.visualDebugging) { }

    protected override Collector<VisualDebuggingEntity> GetTrigger(IContext<VisualDebuggingEntity> context) {
        return context.CreateCollector(Matcher<VisualDebuggingEntity>.AllOf(0));
    }

    protected override bool Filter(VisualDebuggingEntity entity) {
        return true;
    }

    public void Initialize() {
    }

    protected override void Execute(List<VisualDebuggingEntity> entities) {
    }
}
