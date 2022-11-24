//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly PlayerComponent playerComponent = new PlayerComponent();

    public bool isPlayer {
        get { return HasComponent(GameComponentsLookup.Player); }
        set {
            if (value != isPlayer) {
                var index = GameComponentsLookup.Player;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : playerComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherPlayer;

    public static Entitas.IMatcher<GameEntity> Player {
        get {
            if (_matcherPlayer == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Player);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherPlayer = matcher;
            }

            return _matcherPlayer;
        }
    }
}
