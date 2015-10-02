namespace Entitas {
    public partial class Matcher {
        public static IAllOfMatcher AllOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._allOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAllOfMatcher AllOf(params IMatcher[] matchers) {
            return Matcher.AllOf(mergeIndices(matchers));
        }

        public static IAnyOfMatcher AnyOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._anyOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAnyOfMatcher AnyOf(params IMatcher[] matchers) {
            return Matcher.AnyOf(mergeIndices(matchers));
        }
    }
}

