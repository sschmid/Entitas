public partial class GameEntity {

    public AssetComponent asset { get { return (AssetComponent)GetComponent(GameComponentsLookup.Asset); } }
    public bool hasAsset { get { return HasComponent(GameComponentsLookup.Asset); } }

    public void AddAsset(string newName) {
        var index = GameComponentsLookup.Asset;
        var component = (AssetComponent)CreateComponent(index, typeof(AssetComponent));
        component.Name = newName;
        AddComponent(index, component);
    }

    public void ReplaceAsset(string newName) {
        var index = GameComponentsLookup.Asset;
        var component = (AssetComponent)CreateComponent(index, typeof(AssetComponent));
        component.Name = newName;
        ReplaceComponent(index, component);
    }

    public void RemoveAsset() {
        RemoveComponent(GameComponentsLookup.Asset);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherAsset;

    public static Entitas.IMatcher<GameEntity> Asset {
        get {
            if (_matcherAsset == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Asset);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherAsset = matcher;
            }

            return _matcherAsset;
        }
    }
}
