namespace Entitas {
    public partial class Matcher {
        public static IAllOfMatcher AllOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._allOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAllOfMatcher AllOf(params IMatcher[] matchers) {
            var allOfMatcher = (Matcher)Matcher.AllOf(mergeIndices(matchers));
            setComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher AnyOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._anyOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAnyOfMatcher AnyOf(params IMatcher[] matchers) {
            var anyOfMatcher = (Matcher)Matcher.AnyOf(mergeIndices(matchers));
            setComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }
    }
}

