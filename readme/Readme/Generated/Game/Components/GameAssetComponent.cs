public partial class GameEntity
{
    public AssetComponent Asset => (AssetComponent)GetComponent(GameComponentsLookup.Asset);
    public bool HasAsset => HasComponent(GameComponentsLookup.Asset);

    public GameEntity AddAsset(string newName)
    {
        var index = GameComponentsLookup.Asset;
        var component = (AssetComponent)CreateComponent(index, typeof(AssetComponent));
        component.Name = newName;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceAsset(string newName)
    {
        var index = GameComponentsLookup.Asset;
        var component = (AssetComponent)CreateComponent(index, typeof(AssetComponent));
        component.Name = newName;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveAsset()
    {
        RemoveComponent(GameComponentsLookup.Asset);
        return this;
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherAsset;

    public static Entitas.IMatcher<GameEntity> Asset
    {
        get
        {
            if (_matcherAsset == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Asset);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherAsset = matcher;
            }

            return _matcherAsset;
        }
    }
}
