public partial class GameEntity {

    public PositionComponent position { get { return (PositionComponent)GetComponent(GameComponentsLookup.Position); } }
    public bool hasPosition { get { return HasComponent(GameComponentsLookup.Position); } }

    public void AddPosition(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.Position;
        var component = (PositionComponent)CreateComponent(index, typeof(PositionComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplacePosition(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.Position;
        var component = (PositionComponent)CreateComponent(index, typeof(PositionComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemovePosition() {
        RemoveComponent(GameComponentsLookup.Position);
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
