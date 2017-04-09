using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentData : CodeGeneratorData {
    }

    public static class ComponentDataExtension {

        public static string ToComponentName(this string fullTypeName) {
            return shouldIgnoreNamespaces()
                ? fullTypeName.ShortTypeName().RemoveComponentSuffix()
                : fullTypeName.RemoveDots().RemoveComponentSuffix();
        }

        public const string IGNORE_NAMESPACES_KEY = "Entitas.CodeGeneration.Plugins.IgnoreNamespaces";

        static Config _config;

        static bool shouldIgnoreNamespaces() {
            if(_config == null) {
                _config = Preferences.LoadConfig();
            }

            return _config.GetValueOrDefault(IGNORE_NAMESPACES_KEY, "false") == "true";
        }
    }
}
