public partial class GameEntity {

    static readonly InteractiveComponent interactiveComponent = new InteractiveComponent();

    public bool isInteractive {
        get { return HasComponent(GameComponentsLookup.Interactive); }
        set {
            if (value != isInteractive) {
                var index = GameComponentsLookup.Interactive;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : interactiveComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherInteractive;

    public static Entitas.IMatcher<GameEntity> Interactive {
        get {
            if (_matcherInteractive == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Interactive);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherInteractive = matcher;
            }

            return _matcherInteractive;
        }
    }
}
