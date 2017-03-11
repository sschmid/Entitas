public partial class GameEntity {

    public ViewComponent view { get { return (ViewComponent)GetComponent(GameComponentsLookup.View); } }
    public bool hasView { get { return HasComponent(GameComponentsLookup.View); } }

    public void AddView(UnityEngine.GameObject newGameObject) {
        var component = CreateComponent<ViewComponent>(GameComponentsLookup.View);
        component.gameObject = newGameObject;
        AddComponent(GameComponentsLookup.View, component);
    }

    public void ReplaceView(UnityEngine.GameObject newGameObject) {
        var component = CreateComponent<ViewComponent>(GameComponentsLookup.View);
        component.gameObject = newGameObject;
        ReplaceComponent(GameComponentsLookup.View, component);
    }

    public void RemoveView() {
        RemoveComponent(GameComponentsLookup.View);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherView;

    public static Entitas.IMatcher<GameEntity> View {
        get {
            if(_matcherView == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.View);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherView = matcher;
            }

            return _matcherView;
        }
    }
}
