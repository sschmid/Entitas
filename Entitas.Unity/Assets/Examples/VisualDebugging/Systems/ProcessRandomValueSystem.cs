using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem {

    readonly Context _context;

    public ProcessRandomValueSystem(Contexts contexts) : base(contexts.visualDebugging)
    {
        _context = contexts.visualDebugging;
    }

    protected override Collector GetTrigger(Context context) {
        return context.CreateCollector(VisualDebuggingMatcher.MyFloat);
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    protected override void Execute(List<Entity> entities) {
        foreach(var e in entities) {
            _context.DestroyEntity(e);
        }
    }
}
