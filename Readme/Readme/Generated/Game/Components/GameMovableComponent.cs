public partial class GameEntity {

    static readonly MovableComponent movableComponent = new MovableComponent();

    public bool isMovable {
        get { return HasComponent(GameComponentsLookup.Movable); }
        set {
            if(value != isMovable) {
                if(value) {
                    AddComponent(GameComponentsLookup.Movable, movableComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.Movable);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMovable;

    public static Entitas.IMatcher<GameEntity> Movable {
        get {
            if(_matcherMovable == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Movable);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMovable = matcher;
            }

            return _matcherMovable;
        }
    }
}
