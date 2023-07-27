using System.Collections.Generic;
using System.Threading;
using Entitas;

public class AReactiveSystem : ReactiveSystem<Game.Entity>
{
    public AReactiveSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(GameMyStringMatcher.MyString);

    protected override bool Filter(Game.Entity entity) => true;

    protected override void Execute(List<Game.Entity> entities)
    {
        Thread.Sleep(2);
    }
}
