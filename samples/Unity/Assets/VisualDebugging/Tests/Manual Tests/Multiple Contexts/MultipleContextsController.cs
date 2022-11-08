using UnityEngine;
using Entitas;
using Entitas.VisualDebugging.Unity;

public class MultipleContextsController : MonoBehaviour
{
    IContext _contextA;
    IContext _contextB;

    void Start()
    {
        _contextA = new Context<GameEntity>(
            GameComponentsLookup.TotalComponents,
            0,
            new ContextInfo("Context A", GameComponentsLookup.componentNames, GameComponentsLookup.componentTypes),
            entity => new SafeAERC(entity),
            () => new GameEntity()
        );

        new ContextObserver(_contextA);

        _contextB = new Context<GameEntity>(
            GameComponentsLookup.TotalComponents,
            0,
            new ContextInfo("Context B", GameComponentsLookup.componentNames, GameComponentsLookup.componentTypes),
            entity => new SafeAERC(entity),
            () => new GameEntity()
        );

        new ContextObserver(_contextB);

        _contextA.OnEntityCreated += (context, entity) => ((GameEntity)entity).AddMyInt(42);
    }
}
