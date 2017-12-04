namespace Entitas {

    public static class ContextStringExtension {

        const string CONTEXT_SUFFIX = "Context";

        public static string AddContextSuffix(this string contextName) {
            return contextName.EndsWith(CONTEXT_SUFFIX, System.StringComparison.Ordinal)
                ? contextName
                : contextName + CONTEXT_SUFFIX;
        }

        public static string RemoveContextSuffix(this string contextName) {
            return contextName.EndsWith(CONTEXT_SUFFIX, System.StringComparison.Ordinal)
                ? contextName.Substring(0, contextName.Length - CONTEXT_SUFFIX.Length)
                : contextName;
        }
    }
}
