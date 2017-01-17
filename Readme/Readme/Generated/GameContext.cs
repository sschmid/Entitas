namespace Entitas {

    public sealed partial class GameContext : XXXContext<GameEntity> {

        public GameContext() : base(ComponentIds.TotalComponents) {
        }
    }
}
