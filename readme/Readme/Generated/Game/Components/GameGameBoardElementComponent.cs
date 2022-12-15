public partial class GameEntity
{
    static readonly GameBoardElementComponent GameBoardElementComponent = new GameBoardElementComponent();

    public bool IsGameBoardElement
    {
        get => HasComponent(GameComponentsLookup.GameBoardElement);
        set
        {
            if (value != IsGameBoardElement)
            {
                const int index = GameComponentsLookup.GameBoardElement;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : GameBoardElementComponent;

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

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherGameBoardElement;

    public static Entitas.IMatcher<GameEntity> GameBoardElement {
        get {
            if (_matcherGameBoardElement == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameBoardElement);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherGameBoardElement = matcher;
            }

            return _matcherGameBoardElement;
        }
    }
}
