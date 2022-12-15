public partial class GameEntity
{
    static readonly MovableComponent MovableComponent = new MovableComponent();

    public bool IsMovable
    {
        get => HasComponent(GameComponentsLookup.Movable);
        set
        {
            if (value != IsMovable)
            {
                const int index = GameComponentsLookup.Movable;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : MovableComponent;

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
