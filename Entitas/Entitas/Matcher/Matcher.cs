namespace Entitas {
    public static partial class Matcher {
        public static AllOfMatcher AllOf(params int[] indices) {
            return new AllOfMatcher(indices);
        }

        public static AnyOfMatcher AnyOf(params int[] indices) {
            return new AnyOfMatcher(indices);
        }

        // Compound Matcher

        public static AllOfCompoundMatcher AllOf(params IMatcher[] matchers) {
            return new AllOfCompoundMatcher(matchers);
        }

        public static AnyOfCompoundMatcher AnyOf(params IMatcher[] matchers) {
            return new AnyOfCompoundMatcher(matchers);
        }
    }
}
