using System.Collections.Generic;
using Entitas;

public class SomeReactiveSystem : ReactiveSystem<GameEntity> {

    public SomeReactiveSystem(Contexts contexts) : base(contexts.game) { }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(Matcher<GameEntity>.AllOf(0));
    }

    protected override bool Filter(GameEntity entity) {
        return true;
    }

    protected override void Execute(List<GameEntity> entities) {
    }
}
