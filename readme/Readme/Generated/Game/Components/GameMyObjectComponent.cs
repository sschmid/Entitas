public partial class GameEntity
{
    public MyObjectComponent MyObject => (MyObjectComponent)GetComponent(GameComponentsLookup.MyObject);
    public bool HasMyObject => HasComponent(GameComponentsLookup.MyObject);

    public GameEntity AddMyObject(MyObject newValue)
    {
        var index = GameComponentsLookup.MyObject;
        var component = (MyObjectComponent)CreateComponent(index, typeof(MyObjectComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceMyObject(MyObject newValue)
    {
        var index = GameComponentsLookup.MyObject;
        var component = (MyObjectComponent)CreateComponent(index, typeof(MyObjectComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveMyObject()
    {
        RemoveComponent(GameComponentsLookup.MyObject);
        return this;
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherMyObject;

    public static Entitas.IMatcher<GameEntity> MyObject
    {
        get
        {
            if (_matcherMyObject == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MyObject);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherMyObject = matcher;
            }

            return _matcherMyObject;
        }
    }
}
