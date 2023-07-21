using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem<Entity>
{
    public TestReactiveSystem(IContext<Entity> context) : base(context) { }

    protected override ICollector<Entity> GetTrigger(IContext<Entity> context)
    {
        return context.CreateCollector(Matcher<Entity>.AllOf(0));
    }

    protected override bool Filter(Entity entity)
    {
        return true;
    }

    protected override void Execute(List<Entity> entities) { }
}
