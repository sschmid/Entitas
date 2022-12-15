public partial class GameEntity
{
    public VelocityComponent Velocity => (VelocityComponent)GetComponent(GameComponentsLookup.Velocity);
    public bool HasVelocity => HasComponent(GameComponentsLookup.Velocity);

    public GameEntity AddVelocity(UnityEngine.Vector3 newValue)
    {
        var index = GameComponentsLookup.Velocity;
        var component = (VelocityComponent)CreateComponent(index, typeof(VelocityComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceVelocity(UnityEngine.Vector3 newValue)
    {
        var index = GameComponentsLookup.Velocity;
        var component = (VelocityComponent)CreateComponent(index, typeof(VelocityComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveVelocity()
    {
        RemoveComponent(GameComponentsLookup.Velocity);
        return this;
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherVelocity;

    public static Entitas.IMatcher<GameEntity> Velocity
    {
        get
        {
            if (_matcherVelocity == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Velocity);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherVelocity = matcher;
            }

            return _matcherVelocity;
        }
    }
}
