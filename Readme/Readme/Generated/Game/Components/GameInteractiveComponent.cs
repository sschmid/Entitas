public partial class GameEntity {

    static readonly InteractiveComponent interactiveComponent = new InteractiveComponent();

    public bool isInteractive {
        get { return HasComponent(GameComponentsLookup.Interactive); }
        set {
            if(value != isInteractive) {
                if(value) {
                    AddComponent(GameComponentsLookup.Interactive, interactiveComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.Interactive);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherInteractive;

    public static Entitas.IMatcher<GameEntity> Interactive {
        get {
            if(_matcherInteractive == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Interactive);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherInteractive = matcher;
            }

            return _matcherInteractive;
        }
    }
}
