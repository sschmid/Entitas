using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReactiveSystemExceptionController : MonoBehaviour {

    GameEntity _entity;
    ExceptionReactiveSystem _system;

    void Start() {
        var contexts = Contexts.sharedInstance;
        _entity = contexts.game.CreateEntity();
        _system = new ExceptionReactiveSystem(contexts);
    }

    void Update() {
        _entity.ReplaceMyString(Random.value.ToString());
        _system.Execute();
    }
}

public class ExceptionReactiveSystem : ReactiveSystem<GameEntity> {

    public ExceptionReactiveSystem(Contexts contexts) : base(contexts.game) { }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.MyString);
    }

    protected override bool Filter(GameEntity entity) {
        return true;
    }

    protected override void Execute(List<GameEntity> entities) {
        if (Random.value > 0.99f) {
            throw new Exception("ExceptionReactiveSystem Exception!");
        }
    }
}
