public partial class GameEntity
{
    public PositionComponent Position => (PositionComponent)GetComponent(GameComponentsLookup.Position);
    public bool HasPosition => HasComponent(GameComponentsLookup.Position);

    public GameEntity AddPosition(UnityEngine.Vector3 newValue)
    {
        var index = GameComponentsLookup.Position;
        var component = (PositionComponent)CreateComponent(index, typeof(PositionComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplacePosition(UnityEngine.Vector3 newValue)
    {
        var index = GameComponentsLookup.Position;
        var component = (PositionComponent)CreateComponent(index, typeof(PositionComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemovePosition()
    {
        RemoveComponent(GameComponentsLookup.Position);
        return this;
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherPosition;

    public static Entitas.IMatcher<GameEntity> Position {
        get {
            if (_matcherPosition == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Position);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherPosition = matcher;
            }

            return _matcherPosition;
        }
    }
}
