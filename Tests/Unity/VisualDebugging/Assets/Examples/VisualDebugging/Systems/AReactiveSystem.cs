using System.Collections.Generic;
using System.Threading;
using Entitas;
using Entitas.Core;

public class AReactiveSystem : ReactiveSystem<GameEntity> {

    public AReactiveSystem(Contexts contexts) : base(contexts.game) { }

    protected override Collector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.MyString);
    }

    protected override bool Filter(GameEntity entity) {
        return true;
    }

    protected override void Execute(List<GameEntity> entities) {
        Thread.Sleep(2);
    }
}
