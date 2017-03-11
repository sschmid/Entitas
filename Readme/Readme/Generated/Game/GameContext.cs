public sealed partial class GameContext : Entitas.Context<GameEntity> {

    public GameContext()
        : base(
            GameComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Game",
                GameComponentsLookup.componentNames,
                GameComponentsLookup.componentTypes
            )
        ) {
    }
}
