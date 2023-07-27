using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem<Game.Entity>
{
    public ProcessRandomValueSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(GameMyFloatMatcher.MyFloat);

    protected override bool Filter(Game.Entity entity) => true;

    protected override void Execute(List<Game.Entity> entities)
    {
        foreach (var entity in entities)
            entity.Destroy();
    }
}
