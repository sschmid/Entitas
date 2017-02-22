public partial class GameEntity {

    public NameComponent name { get { return (NameComponent)GetComponent(GameComponentsLookup.Name); } }
    public bool hasName { get { return HasComponent(GameComponentsLookup.Name); } }

    public void AddName(string newValue) {
        var component = CreateComponent<NameComponent>(GameComponentsLookup.Name);
        component.value = newValue;
        AddComponent(GameComponentsLookup.Name, component);
    }

    public void ReplaceName(string newValue) {
        var component = CreateComponent<NameComponent>(GameComponentsLookup.Name);
        component.value = newValue;
        ReplaceComponent(GameComponentsLookup.Name, component);
    }

    public void RemoveName() {
        RemoveComponent(GameComponentsLookup.Name);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherName;

    public static Entitas.IMatcher<GameEntity> Name {
        get {
            if(_matcherName == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Name);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherName = matcher;
            }

            return _matcherName;
        }
    }
}
