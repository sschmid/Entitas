using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem<GameEntity>
{
    public ProcessRandomValueSystem(Contexts contexts) : base(contexts.game) { }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.MyFloat);

    protected override bool Filter(GameEntity entity) => true;

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
            e.Destroy();
    }
}
