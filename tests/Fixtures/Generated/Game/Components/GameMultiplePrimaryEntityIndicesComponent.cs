//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public MultiplePrimaryEntityIndexesComponent multiplePrimaryEntityIndexes { get { return (MultiplePrimaryEntityIndexesComponent)GetComponent(GameComponentsLookup.MultiplePrimaryEntityIndexes); } }
    public bool hasMultiplePrimaryEntityIndexes { get { return HasComponent(GameComponentsLookup.MultiplePrimaryEntityIndexes); } }

    public void AddMultiplePrimaryEntityIndexes(string newValue, string newValue2) {
        var index = GameComponentsLookup.MultiplePrimaryEntityIndexes;
        var component = (MultiplePrimaryEntityIndexesComponent)CreateComponent(index, typeof(MultiplePrimaryEntityIndexesComponent));
        component.value = newValue;
        component.value2 = newValue2;
        AddComponent(index, component);
    }

    public void ReplaceMultiplePrimaryEntityIndexes(string newValue, string newValue2) {
        var index = GameComponentsLookup.MultiplePrimaryEntityIndexes;
        var component = (MultiplePrimaryEntityIndexesComponent)CreateComponent(index, typeof(MultiplePrimaryEntityIndexesComponent));
        component.value = newValue;
        component.value2 = newValue2;
        ReplaceComponent(index, component);
    }

    public void RemoveMultiplePrimaryEntityIndexes() {
        RemoveComponent(GameComponentsLookup.MultiplePrimaryEntityIndexes);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMultiplePrimaryEntityIndexes;

    public static Entitas.IMatcher<GameEntity> MultiplePrimaryEntityIndexes {
        get {
            if (_matcherMultiplePrimaryEntityIndexes == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MultiplePrimaryEntityIndexes);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMultiplePrimaryEntityIndexes = matcher;
            }

            return _matcherMultiplePrimaryEntityIndexes;
        }
    }
}
