namespace Entitas {

    public static class EntityStringExtension {

        public const string ENTITY_SUFFIX = "Entity";

        public static string AddEntitySuffix(this string str) {
            return str.EndsWith(ENTITY_SUFFIX, System.StringComparison.Ordinal)
                ? str
                : str + ENTITY_SUFFIX;
        }

        public static string RemoveEntitySuffix(this string str) {
            return str.EndsWith(ENTITY_SUFFIX, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - ENTITY_SUFFIX.Length)
                : str;
        }
    }
}
