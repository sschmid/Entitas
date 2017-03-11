public partial class GameEntity {

    public PositionComponent position { get { return (PositionComponent)GetComponent(GameComponentsLookup.Position); } }
    public bool hasPosition { get { return HasComponent(GameComponentsLookup.Position); } }

    public void AddPosition(UnityEngine.Vector3 newValue) {
        var component = CreateComponent<PositionComponent>(GameComponentsLookup.Position);
        component.value = newValue;
        AddComponent(GameComponentsLookup.Position, component);
    }

    public void ReplacePosition(UnityEngine.Vector3 newValue) {
        var component = CreateComponent<PositionComponent>(GameComponentsLookup.Position);
        component.value = newValue;
        ReplaceComponent(GameComponentsLookup.Position, component);
    }

    public void RemovePosition() {
        RemoveComponent(GameComponentsLookup.Position);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherPosition;

    public static Entitas.IMatcher<GameEntity> Position {
        get {
            if(_matcherPosition == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Position);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherPosition = matcher;
            }

            return _matcherPosition;
        }
    }
}
