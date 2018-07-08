namespace Entitas {

    public static class ListenerStringExtension {

        public const string LISTENER_SUFFIX = "Listener";

        public static string AddListenerSuffix(this string str) {
            return str.EndsWith(LISTENER_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + LISTENER_SUFFIX;
        }

        public static string RemoveListenerSuffix(this string str) {
            return str.EndsWith(LISTENER_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - LISTENER_SUFFIX.Length)
                : str;
        }
    }
}
