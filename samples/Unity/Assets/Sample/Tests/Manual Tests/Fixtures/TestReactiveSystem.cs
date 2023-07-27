using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem<Game.Entity>
{
    public TestReactiveSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(GameTestMatcher.Test);

    protected override bool Filter(Game.Entity entity) => true;

    protected override void Execute(List<Game.Entity> entities) { }
}
