public sealed partial class GameMatcher
{
    public static Entitas.IAllOfMatcher<GameEntity> AllOf(params int[] indexes) =>
        Entitas.Matcher<GameEntity>.AllOf(indexes);

    public static Entitas.IAllOfMatcher<GameEntity> AllOf(params Entitas.IMatcher<GameEntity>[] matchers) =>
        Entitas.Matcher<GameEntity>.AllOf(matchers);

    public static Entitas.IAnyOfMatcher<GameEntity> AnyOf(params int[] indexes) =>
        Entitas.Matcher<GameEntity>.AnyOf(indexes);

    public static Entitas.IAnyOfMatcher<GameEntity> AnyOf(params Entitas.IMatcher<GameEntity>[] matchers) =>
        Entitas.Matcher<GameEntity>.AnyOf(matchers);
}
