public sealed partial class GameContext : Entitas.Context<GameEntity>
{
    public GameContext() : base(
        GameComponentsLookup.TotalComponents,
        0,
        new Entitas.ContextInfo(
            "Game",
            GameComponentsLookup.ComponentNames,
            GameComponentsLookup.ComponentTypes
        ),
        entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
            new Entitas.UnsafeAERC(),
#else
            new Entitas.SafeAERC(entity),
#endif
        () => new GameEntity()
    ) { }
}
