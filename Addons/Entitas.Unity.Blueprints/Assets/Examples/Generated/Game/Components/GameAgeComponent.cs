public partial class GameEntity {

    public AgeComponent age { get { return (AgeComponent)GetComponent(GameComponentsLookup.Age); } }
    public bool hasAge { get { return HasComponent(GameComponentsLookup.Age); } }

    public void AddAge(int newValue) {
        var component = CreateComponent<AgeComponent>(GameComponentsLookup.Age);
        component.value = newValue;
        AddComponent(GameComponentsLookup.Age, component);
    }

    public void ReplaceAge(int newValue) {
        var component = CreateComponent<AgeComponent>(GameComponentsLookup.Age);
        component.value = newValue;
        ReplaceComponent(GameComponentsLookup.Age, component);
    }

    public void RemoveAge() {
        RemoveComponent(GameComponentsLookup.Age);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherAge;

    public static Entitas.IMatcher<GameEntity> Age {
        get {
            if(_matcherAge == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Age);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAge = matcher;
            }

            return _matcherAge;
        }
    }
}
