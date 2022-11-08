using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem<GameEntity>
{
    public TestReactiveSystem(Contexts contexts) : base(contexts.game) { }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.Test);

    protected override bool Filter(GameEntity entity) => true;

    protected override void Execute(List<GameEntity> entities) { }
}
