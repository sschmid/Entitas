namespace Entitas {

    public static class EntityExtension {

        public const string COMPONENT_SUFFIX = "Component";

        public static string AddComponentSuffix(this string componentName) {
            return componentName.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? componentName
                : componentName + COMPONENT_SUFFIX;
        }

        public static string RemoveComponentSuffix(this string componentName) {
            return componentName.EndsWith(COMPONENT_SUFFIX, System.StringComparison.Ordinal)
                ? componentName.Substring(0, componentName.Length - COMPONENT_SUFFIX.Length)
                : componentName;
        }
    }
}
