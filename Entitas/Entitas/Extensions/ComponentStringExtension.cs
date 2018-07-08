namespace Entitas {

    public static class ComponentStringExtension {

        public const string COMPONENT_SUFFIX = "Component";

        public static string AddComponentSuffix(this string str) {
            return str.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + COMPONENT_SUFFIX;
        }

        public static string RemoveComponentSuffix(this string str) {
            return str.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - COMPONENT_SUFFIX.Length)
                : str;
        }
    }
}
