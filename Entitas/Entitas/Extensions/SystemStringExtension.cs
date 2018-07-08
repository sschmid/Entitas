namespace Entitas {

    public static class SystemStringExtension {

        const string SYSTEM_SUFFIX = "System";

        public static string AddSystemSuffix(this string str) {
            return str.EndsWith(SYSTEM_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + SYSTEM_SUFFIX;
        }

        public static string RemoveSystemSuffix(this string str) {
            return str.EndsWith(SYSTEM_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - SYSTEM_SUFFIX.Length)
                : str;
        }
    }
}
