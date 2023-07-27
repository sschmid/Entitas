using System.Collections.Generic;
using Entitas;

public class SomeInitializeReactiveSystem : ReactiveSystem<Game.Entity>, IInitializeSystem
{
    public SomeInitializeReactiveSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(Game.Matcher.AllOf(0));

    protected override bool Filter(Game.Entity entity) => true;

    public void Initialize() { }

    protected override void Execute(List<Game.Entity> entities) { }
}
