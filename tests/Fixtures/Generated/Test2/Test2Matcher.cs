//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class Test2Matcher {

    public static Entitas.IAllOfMatcher<Test2Entity> AllOf(params int[] indexes) {
        return Entitas.Matcher<Test2Entity>.AllOf(indexes);
    }

    public static Entitas.IAllOfMatcher<Test2Entity> AllOf(params Entitas.IMatcher<Test2Entity>[] matchers) {
          return Entitas.Matcher<Test2Entity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<Test2Entity> AnyOf(params int[] indexes) {
          return Entitas.Matcher<Test2Entity>.AnyOf(indexes);
    }

    public static Entitas.IAnyOfMatcher<Test2Entity> AnyOf(params Entitas.IMatcher<Test2Entity>[] matchers) {
          return Entitas.Matcher<Test2Entity>.AnyOf(matchers);
    }
}
