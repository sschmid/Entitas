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

public partial class GameEntity {

    static readonly AnimatingComponent animatingComponent = new AnimatingComponent();

    public bool IsAnimating {
        get { return HasComponent(GameComponentsLookup.Animating); }
        set {
            if (value != IsAnimating) {
                var index = GameComponentsLookup.Animating;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : animatingComponent;

                    AddComponent(index, component);
                } else {
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
