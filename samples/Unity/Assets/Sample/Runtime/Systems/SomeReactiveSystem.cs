using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : ReactiveSystem<Game.Entity>
{
    public SomeReactiveSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(Matcher<Game.Entity>.AllOf(0));

    protected override bool Filter(Game.Entity entity) => true;

    protected override void Execute(List<Game.Entity> entities) { }
}
