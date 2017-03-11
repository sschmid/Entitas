public partial class GameEntity {

    static readonly GameBoardElementComponent gameBoardElementComponent = new GameBoardElementComponent();

    public bool isGameBoardElement {
        get { return HasComponent(GameComponentsLookup.GameBoardElement); }
        set {
            if(value != isGameBoardElement) {
                if(value) {
                    AddComponent(GameComponentsLookup.GameBoardElement, gameBoardElementComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.GameBoardElement);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherGameBoardElement;

    public static Entitas.IMatcher<GameEntity> GameBoardElement {
        get {
            if(_matcherGameBoardElement == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameBoardElement);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGameBoardElement = matcher;
            }

            return _matcherGameBoardElement;
        }
    }
}
