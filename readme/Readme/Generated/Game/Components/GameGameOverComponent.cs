public partial class GameEntity {

    static readonly GameOverComponent gameOverComponent = new GameOverComponent();

    public bool IsGameOver {
        get { return HasComponent(GameComponentsLookup.GameOver); }
        set {
            if (value != IsGameOver) {
                var index = GameComponentsLookup.GameOver;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : gameOverComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherGameOver;

    public static Entitas.IMatcher<GameEntity> GameOver {
        get {
            if (_matcherGameOver == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameOver);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherGameOver = matcher;
            }

            return _matcherGameOver;
        }
    }
}
