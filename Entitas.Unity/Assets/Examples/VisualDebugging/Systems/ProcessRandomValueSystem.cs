using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem<VisualDebuggingEntity> {

    readonly IContext<VisualDebuggingEntity> _context;

    public ProcessRandomValueSystem(Contexts contexts) : base(contexts.visualDebugging)
    {
        _context = contexts.visualDebugging;
    }

    protected override Collector<VisualDebuggingEntity> GetTrigger(IContext<VisualDebuggingEntity> context) {
        return context.CreateCollector(VisualDebuggingMatcher.MyFloat);
    }

    protected override bool Filter(VisualDebuggingEntity entity) {
        return true;
    }

    protected override void Execute(List<VisualDebuggingEntity> entities) {
        foreach(var e in entities) {
            _context.DestroyEntity(e);
        }
    }
}
