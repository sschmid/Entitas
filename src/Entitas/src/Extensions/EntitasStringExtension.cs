namespace Entitas
{
    public static class EntitasStringExtension
    {
        public const string CONTEXT_SUFFIX = "Context";
        public const string ENTITY_SUFFIX = "Entity";
        public const string COMPONENT_SUFFIX = "Component";
        public const string SYSTEM_SUFFIX = "System";
        public const string MATCHER_SUFFIX = "Matcher";
        public const string LISTENER_SUFFIX = "Listener";

        public static string AddContextSuffix(this string str) => addSuffix(str, CONTEXT_SUFFIX);
        public static string RemoveContextSuffix(this string str) => removeSuffix(str, CONTEXT_SUFFIX);
        public static bool HasContextSuffix(this string str) => hasSuffix(str, CONTEXT_SUFFIX);

        public static string AddEntitySuffix(this string str) => addSuffix(str, ENTITY_SUFFIX);
        public static string RemoveEntitySuffix(this string str) => removeSuffix(str, ENTITY_SUFFIX);
        public static bool HasEntitySuffix(this string str) => hasSuffix(str, ENTITY_SUFFIX);

        public static string AddComponentSuffix(this string str) => addSuffix(str, COMPONENT_SUFFIX);
        public static string RemoveComponentSuffix(this string str) => removeSuffix(str, COMPONENT_SUFFIX);
        public static bool HasComponentSuffix(this string str) => hasSuffix(str, COMPONENT_SUFFIX);

        public static string AddSystemSuffix(this string str) => addSuffix(str, SYSTEM_SUFFIX);
        public static string RemoveSystemSuffix(this string str) => removeSuffix(str, SYSTEM_SUFFIX);
        public static bool HasSystemSuffix(this string str) => hasSuffix(str, SYSTEM_SUFFIX);

        public static string AddMatcherSuffix(this string str) => addSuffix(str, MATCHER_SUFFIX);
        public static string RemoveMatcherSuffix(this string str) => removeSuffix(str, MATCHER_SUFFIX);
        public static bool HasMatcherSuffix(this string str) => hasSuffix(str, MATCHER_SUFFIX);

        public static string AddListenerSuffix(this string str) => addSuffix(str, LISTENER_SUFFIX);
        public static string RemoveListenerSuffix(this string str) => removeSuffix(str, LISTENER_SUFFIX);
        public static bool HasListenerSuffix(this string str) => hasSuffix(str, LISTENER_SUFFIX);

        static string addSuffix(string str, string suffix) => hasSuffix(str, suffix) ? str : str + suffix;
        static string removeSuffix(string str, string suffix) => hasSuffix(str, suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        static bool hasSuffix(string str, string suffix) => str.EndsWith(suffix, System.StringComparison.Ordinal);
    }
}
