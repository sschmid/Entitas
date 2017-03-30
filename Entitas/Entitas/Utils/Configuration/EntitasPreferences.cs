using System.IO;

namespace Entitas {

    public static class EntitasPreferences {

        static string configPath = "Entitas.properties";

        public static void SetConfigPath(string configPath) {
            EntitasPreferences.configPath = configPath;
        }

        public static string GetConfigPath() {
            return configPath;
        }

        public static EntitasPreferencesConfig LoadConfig(string configPath) {
            SetConfigPath(configPath);

            var config = File.Exists(configPath)
                             ? File.ReadAllText(configPath)
                             : string.Empty;

            return new EntitasPreferencesConfig(config);
        }

        public static void SaveConfig(EntitasPreferencesConfig config) {
            File.WriteAllText(configPath, config.ToString());
        }
    }
}
