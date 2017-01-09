using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem {

    readonly Context _context;

    public ProcessRandomValueSystem(Contexts contexts) : base(
        contexts.visualDebugging.CreateCollector(VisualDebuggingMatcher.MyFloat))
    {
        _context = contexts.visualDebugging;
    }

    protected override bool Filter(Entity entity) {
        return true;
    }

    public override void Execute(List<Entity> entities) {
        foreach(var e in entities) {
            _context.DestroyEntity(e);
        }
    }
}
