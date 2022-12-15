public partial class GameEntity {

    static readonly MovableComponent movableComponent = new MovableComponent();

    public bool IsMovable {
        get { return HasComponent(GameComponentsLookup.Movable); }
        set {
            if (value != IsMovable) {
                var index = GameComponentsLookup.Movable;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : movableComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMovable;

    public static Entitas.IMatcher<GameEntity> Movable {
        get {
            if (_matcherMovable == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Movable);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherMovable = matcher;
            }

            return _matcherMovable;
        }
    }
}
