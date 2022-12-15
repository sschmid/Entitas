public partial class GameEntity
{
    public ViewComponent View => (ViewComponent)GetComponent(GameComponentsLookup.View);
    public bool HasView => HasComponent(GameComponentsLookup.View);

    public GameEntity AddView(UnityEngine.GameObject newGameObject)
    {
        var index = GameComponentsLookup.View;
        var component = (ViewComponent)CreateComponent(index, typeof(ViewComponent));
        component.GameObject = newGameObject;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceView(UnityEngine.GameObject newGameObject)
    {
        var index = GameComponentsLookup.View;
        var component = (ViewComponent)CreateComponent(index, typeof(ViewComponent));
        component.GameObject = newGameObject;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveView()
    {
        RemoveComponent(GameComponentsLookup.View);
        return this;
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherView;

    public static Entitas.IMatcher<GameEntity> View
    {
        get
        {
            if (_matcherView == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.View);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherView = matcher;
            }

            return _matcherView;
        }
    }
}
