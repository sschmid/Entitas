using System;
using System.Collections.Generic;
using System.Globalization;
using Entitas;
using Entitas.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReactiveSystemExceptionController : MonoBehaviour
{
    Game.Entity _entity;
    ExceptionReactiveSystem _system;

    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        var gameContext = new GameContext();
        gameContext.CreateContextObserver();
        _entity = gameContext.CreateEntity();
        _system = new ExceptionReactiveSystem(gameContext);
    }

    void Update()
    {
        _entity.ReplaceMyString(Random.value.ToString(CultureInfo.InvariantCulture));
        _system.Execute();
    }
}

public class ExceptionReactiveSystem : ReactiveSystem<Game.Entity>
{
    public ExceptionReactiveSystem(GameContext context) : base(context) { }

    protected override ICollector<Game.Entity> GetTrigger(IContext<Game.Entity> context) =>
        context.CreateCollector(GameMyStringMatcher.MyString);

    protected override bool Filter(Game.Entity entity) => true;

    protected override void Execute(List<Game.Entity> entities)
    {
        if (Random.value > 0.99f)
            throw new Exception("ExceptionReactiveSystem Exception!");
    }
}
