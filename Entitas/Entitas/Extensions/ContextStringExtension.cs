namespace Entitas {

    public static class ContextStringExtension {

        const string CONTEXT_SUFFIX = "Context";

        public static string AddContextSuffix(this string str) {
            return str.EndsWith(CONTEXT_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + CONTEXT_SUFFIX;
        }

        public static string RemoveContextSuffix(this string str) {
            return str.EndsWith(CONTEXT_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - CONTEXT_SUFFIX.Length)
                : str;
        }
    }
}
