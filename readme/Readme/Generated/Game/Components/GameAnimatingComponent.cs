public partial class GameContext
{
    public GameEntity AnimatingEntity => GetGroup(GameMatcher.Animating).GetSingleEntity();

    public bool IsAnimating
    {
        get => AnimatingEntity != null;
        set
        {
            var entity = AnimatingEntity;
            if (value != (entity != null))
            {
                if (value)
                    CreateEntity().IsAnimating = true;
                else
                    entity.Destroy();
            }
        }
    }
}

public partial class GameEntity
{
    static readonly AnimatingComponent AnimatingComponent = new AnimatingComponent();

    public bool IsAnimating
    {
        get => HasComponent(GameComponentsLookup.Animating);
        set
        {
            if (value != IsAnimating)
            {
                const int index = GameComponentsLookup.Animating;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : AnimatingComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherAnimating;

    public static Entitas.IMatcher<GameEntity> Animating {
        get {
            if (_matcherAnimating == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Animating);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherAnimating = matcher;
            }

            return _matcherAnimating;
        }
    }
}
