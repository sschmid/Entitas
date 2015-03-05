namespace Entitas {
    public static partial class Matcher {
        public static AllOfMatcher AllOf(params int[] indices) {
            return new AllOfMatcher(indices);
        }

        public static AnyOfMatcher AnyOf(params int[] indices) {
            return new AnyOfMatcher(indices);
        }

        public static NoneOfMatcher NoneOf(params int[] indices) {
            return new NoneOfMatcher(indices);
        }

        // Compound Matcher

        public static AllOfCompoundMatcher AllOf(params IMatcher[] matchers) {
            return new AllOfCompoundMatcher(matchers);
        }

        public static AnyOfCompoundMatcher AnyOf(params IMatcher[] matchers) {
            return new AnyOfCompoundMatcher(matchers);
        }
        
        public static NoneOfCompoundMatcher NoneOf(params IMatcher[] matchers) {
            return new NoneOfCompoundMatcher(matchers);
        }
    }
}
