using System.Collections.Generic;
using Entitas;

public class ProcessRandomValueSystem : ReactiveSystem<GameEntity> {

    readonly GameContext _context;

    public ProcessRandomValueSystem(Contexts contexts) : base(contexts.game) {
        _context = contexts.game;
    }

    protected override Collector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.MyFloat);
    }

    protected override bool Filter(GameEntity entity) {
        return true;
    }

    protected override void Execute(List<GameEntity> entities) {
        foreach(var e in entities) {
            _context.DestroyEntity(e);
        }
    }
}
