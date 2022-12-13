public partial class GameEntity {

    public HealthComponent health { get { return (HealthComponent)GetComponent(GameComponentsLookup.Health); } }
    public bool hasHealth { get { return HasComponent(GameComponentsLookup.Health); } }

    public void AddHealth(int newValue) {
        var index = GameComponentsLookup.Health;
        var component = (HealthComponent)CreateComponent(index, typeof(HealthComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceHealth(int newValue) {
        var index = GameComponentsLookup.Health;
        var component = (HealthComponent)CreateComponent(index, typeof(HealthComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveHealth() {
        RemoveComponent(GameComponentsLookup.Health);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherHealth;

    public static Entitas.IMatcher<GameEntity> Health {
        get {
            if (_matcherHealth == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Health);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherHealth = matcher;
            }

            return _matcherHealth;
        }
    }
}
