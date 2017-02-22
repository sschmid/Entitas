public sealed partial class GameStateContext : Entitas.Context<GameStateEntity> {

    public GameStateContext()
        : base(
            GameStateComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "GameState",
                GameStateComponentsLookup.componentNames,
                GameStateComponentsLookup.componentTypes
            )
        ) {
    }
}
