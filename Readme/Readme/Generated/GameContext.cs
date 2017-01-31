namespace Entitas {

    public sealed partial class GameContext : Context<GameEntity> {

        public GameContext() : base(ComponentIds.TotalComponents) {
        }
    }
}
