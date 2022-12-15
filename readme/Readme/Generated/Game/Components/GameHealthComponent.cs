public partial class GameEntity
{
    public HealthComponent Health => (HealthComponent)GetComponent(GameComponentsLookup.Health);
    public bool HasHealth => HasComponent(GameComponentsLookup.Health);

    public GameEntity AddHealth(int newValue)
    {
        var index = GameComponentsLookup.Health;
        var component = (HealthComponent)CreateComponent(index, typeof(HealthComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceHealth(int newValue)
    {
        var index = GameComponentsLookup.Health;
        var component = (HealthComponent)CreateComponent(index, typeof(HealthComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveHealth()
    {
        RemoveComponent(GameComponentsLookup.Health);
        return this;
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
