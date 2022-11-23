namespace Entitas
{
    public static class EntitasStringExtension
    {
        public const string ContextSuffix = "Context";
        public const string EntitySuffix = "Entity";
        public const string ComponentSuffix = "Component";
        public const string SystemSuffix = "System";
        public const string MatcherSuffix = "Matcher";
        public const string ListenerSuffix = "Listener";

        public static string AddContextSuffix(this string str) => AddSuffix(str, ContextSuffix);
        public static string RemoveContextSuffix(this string str) => RemoveSuffix(str, ContextSuffix);
        public static bool HasContextSuffix(this string str) => HasSuffix(str, ContextSuffix);

        public static string AddEntitySuffix(this string str) => AddSuffix(str, EntitySuffix);
        public static string RemoveEntitySuffix(this string str) => RemoveSuffix(str, EntitySuffix);
        public static bool HasEntitySuffix(this string str) => HasSuffix(str, EntitySuffix);

        public static string AddComponentSuffix(this string str) => AddSuffix(str, ComponentSuffix);
        public static string RemoveComponentSuffix(this string str) => RemoveSuffix(str, ComponentSuffix);
        public static bool HasComponentSuffix(this string str) => HasSuffix(str, ComponentSuffix);

        public static string AddSystemSuffix(this string str) => AddSuffix(str, SystemSuffix);
        public static string RemoveSystemSuffix(this string str) => RemoveSuffix(str, SystemSuffix);
        public static bool HasSystemSuffix(this string str) => HasSuffix(str, SystemSuffix);

        public static string AddMatcherSuffix(this string str) => AddSuffix(str, MatcherSuffix);
        public static string RemoveMatcherSuffix(this string str) => RemoveSuffix(str, MatcherSuffix);
        public static bool HasMatcherSuffix(this string str) => HasSuffix(str, MatcherSuffix);

        public static string AddListenerSuffix(this string str) => AddSuffix(str, ListenerSuffix);
        public static string RemoveListenerSuffix(this string str) => RemoveSuffix(str, ListenerSuffix);
        public static bool HasListenerSuffix(this string str) => HasSuffix(str, ListenerSuffix);

        static string AddSuffix(string str, string suffix) => HasSuffix(str, suffix) ? str : str + suffix;
        static string RemoveSuffix(string str, string suffix) => HasSuffix(str, suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        static bool HasSuffix(string str, string suffix) => str.EndsWith(suffix, System.StringComparison.Ordinal);
    }
}
