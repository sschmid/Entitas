using System.IO;

namespace Entitas.Utils {

    public static class Preferences {

        public static string configPath {
            get { return _configPath ?? "Entitas.properties"; }
            set { _configPath = value; }
        }

        static string _configPath;

        // TODO Config delete
        public static Config LoadConfig() {
            var config = File.Exists(configPath)
                             ? File.ReadAllText(configPath)
                             : string.Empty;

            return new Config(config);
        }

        public static Properties LoadConfigProperties() {
            var config = File.Exists(configPath)
                             ? File.ReadAllText(configPath)
                             : string.Empty;

            return new Properties(config);
        }

        public static void SaveConfig(Config config) {
            File.WriteAllText(configPath, config.ToString());
        }
    }
}
