public partial class GameEntity
{
    static readonly PlayerComponent PlayerComponent = new PlayerComponent();

    public bool IsPlayer
    {
        get => HasComponent(GameComponentsLookup.Player);
        set
        {
            if (value != IsPlayer)
            {
                const int index = GameComponentsLookup.Player;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : PlayerComponent;

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
    static Entitas.IMatcher<GameEntity> _matcherPlayer;

    public static Entitas.IMatcher<GameEntity> Player
    {
        get
        {
            if (_matcherPlayer == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Player);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherPlayer = matcher;
            }

            return _matcherPlayer;
        }
    }
}
