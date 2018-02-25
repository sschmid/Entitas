namespace Entitas {

    public static class SystemStringExtension {

        const string COMPONENT_SUFFIX = "System";

        public static string AddSystemSuffix(this string str) {
            return str.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + COMPONENT_SUFFIX;
        }

        public static string RemoveSystemSuffix(this string str) {
            return str.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - COMPONENT_SUFFIX.Length)
                : str;
        }
    }
}
