//public partial class GameEntity {

//    static readonly NoContextComponent noContextComponent = new NoContextComponent();

//    public bool isNoContext {
//        get { return HasComponent(GameComponentsLookup.NoContext); }
//        set {
//            if(value != isNoContext) {
//                if(value) {
//                    AddComponent(GameComponentsLookup.NoContext, noContextComponent);
//                } else {
//                    RemoveComponent(GameComponentsLookup.NoContext);
//                }
//            }
//        }
//    }
//}

//public sealed partial class GameMatcher {

//    static Entitas.IMatcher<GameEntity> _matcherNoContext;

//    public static Entitas.IMatcher<GameEntity> NoContext {
//        get {
//            if(_matcherNoContext == null) {
//                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NoContext);
//                matcher.componentNames = GameComponentsLookup.componentNames;
//                _matcherNoContext = matcher;
//            }

//            return _matcherNoContext;
//        }
//    }
//}
