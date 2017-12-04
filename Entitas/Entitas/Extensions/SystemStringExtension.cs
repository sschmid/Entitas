namespace Entitas {

    public static class SystemStringExtension {

        const string COMPONENT_SUFFIX = "System";

        public static string AddSystemSuffix(this string componentName) {
            return componentName.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? componentName
                : componentName + COMPONENT_SUFFIX;
        }

        public static string RemoveSystemSuffix(this string componentName) {
            return componentName.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? componentName.Substring(0, componentName.Length - COMPONENT_SUFFIX.Length)
                : componentName;
        }
    }
}
