public sealed partial class GameStateContext : Entitas.Context<GameStateEntity>
{
    public GameStateContext() : base(
        GameStateComponentsLookup.TotalComponents,
        0,
        new Entitas.ContextInfo(
            "GameState",
            GameStateComponentsLookup.ComponentNames,
            GameStateComponentsLookup.ComponentTypes
        ),
        entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
            new Entitas.UnsafeAERC(),
#else
            new Entitas.SafeAERC(entity),
#endif
        () => new GameStateEntity()
    ) { }
}
