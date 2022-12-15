public partial class GameEntity
{
    static readonly GameOverComponent GameOverComponent = new GameOverComponent();

    public bool IsGameOver
    {
        get => HasComponent(GameComponentsLookup.GameOver);
        set
        {
            if (value != IsGameOver)
            {
                const int index = GameComponentsLookup.GameOver;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : GameOverComponent;

                    AddComponent(index, component);
                }
                else
                {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherGameOver;

    public static Entitas.IMatcher<GameEntity> GameOver
    {
        get
        {
            if (_matcherGameOver == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameOver);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherGameOver = matcher;
            }

            return _matcherGameOver;
        }
    }
}
