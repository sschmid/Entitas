namespace Entitas {

    public static class MatcherStringExtension {

        public const string MATCHER_SUFFIX = "Matcher";

        public static string AddMatcherSuffix(this string str) {
            return str.EndsWith(MATCHER_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + MATCHER_SUFFIX;
        }

        public static string RemoveMatcherSuffix(this string str) {
            return str.EndsWith(MATCHER_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - MATCHER_SUFFIX.Length)
                : str;
        }
    }
}
