public sealed partial class InputMatcher {

    public static Entitas.IAllOfMatcher<InputEntity> AllOf(params int[] indexes) {
        return Entitas.Matcher<InputEntity>.AllOf(indexes);
    }

    public static Entitas.IAllOfMatcher<InputEntity> AllOf(params Entitas.IMatcher<InputEntity>[] matchers) {
          return Entitas.Matcher<InputEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<InputEntity> AnyOf(params int[] indexes) {
          return Entitas.Matcher<InputEntity>.AnyOf(indexes);
    }

    public static Entitas.IAnyOfMatcher<InputEntity> AnyOf(params Entitas.IMatcher<InputEntity>[] matchers) {
          return Entitas.Matcher<InputEntity>.AnyOf(matchers);
    }
}
