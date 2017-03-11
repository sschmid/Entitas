public partial class GameEntity {

    public AssetComponent asset { get { return (AssetComponent)GetComponent(GameComponentsLookup.Asset); } }
    public bool hasAsset { get { return HasComponent(GameComponentsLookup.Asset); } }

    public void AddAsset(string newName) {
        var component = CreateComponent<AssetComponent>(GameComponentsLookup.Asset);
        component.name = newName;
        AddComponent(GameComponentsLookup.Asset, component);
    }

    public void ReplaceAsset(string newName) {
        var component = CreateComponent<AssetComponent>(GameComponentsLookup.Asset);
        component.name = newName;
        ReplaceComponent(GameComponentsLookup.Asset, component);
    }

    public void RemoveAsset() {
        RemoveComponent(GameComponentsLookup.Asset);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherAsset;

    public static Entitas.IMatcher<GameEntity> Asset {
        get {
            if(_matcherAsset == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Asset);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAsset = matcher;
            }

            return _matcherAsset;
        }
    }
}
