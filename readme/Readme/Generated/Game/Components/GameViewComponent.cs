public partial class GameEntity {

    public ViewComponent view { get { return (ViewComponent)GetComponent(GameComponentsLookup.View); } }
    public bool hasView { get { return HasComponent(GameComponentsLookup.View); } }

    public void AddView(UnityEngine.GameObject newGameObject) {
        var index = GameComponentsLookup.View;
        var component = (ViewComponent)CreateComponent(index, typeof(ViewComponent));
        component.GameObject = newGameObject;
        AddComponent(index, component);
    }

    public void ReplaceView(UnityEngine.GameObject newGameObject) {
        var index = GameComponentsLookup.View;
        var component = (ViewComponent)CreateComponent(index, typeof(ViewComponent));
        component.GameObject = newGameObject;
        ReplaceComponent(index, component);
    }

    public void RemoveView() {
        RemoveComponent(GameComponentsLookup.View);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherView;

    public static Entitas.IMatcher<GameEntity> View {
        get {
            if (_matcherView == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.View);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherView = matcher;
            }

            return _matcherView;
        }
    }
}
