//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameStateMatcher {

    public static Entitas.IAllOfMatcher<GameStateEntity> AllOf(params int[] indexes) {
        return Entitas.Matcher<GameStateEntity>.AllOf(indexes);
    }

    public static Entitas.IAllOfMatcher<GameStateEntity> AllOf(params Entitas.IMatcher<GameStateEntity>[] matchers) {
          return Entitas.Matcher<GameStateEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<GameStateEntity> AnyOf(params int[] indexes) {
          return Entitas.Matcher<GameStateEntity>.AnyOf(indexes);
    }

    public static Entitas.IAnyOfMatcher<GameStateEntity> AnyOf(params Entitas.IMatcher<GameStateEntity>[] matchers) {
          return Entitas.Matcher<GameStateEntity>.AnyOf(matchers);
    }
}
