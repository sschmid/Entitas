using System.IO;

namespace Entitas.Utils {

    public static class Preferences {

        public static string configPath {
            get { return _configPath ?? "Entitas.properties"; }
            set { _configPath = value; }
        }

        static string _configPath;

        public static Properties LoadProperties() {
            var config = File.Exists(configPath)
                             ? File.ReadAllText(configPath)
                             : string.Empty;

            return new Properties(config);
        }

        public static void SaveProperties(Properties properties) {
            File.WriteAllText(configPath, properties.ToString());
        }
    }
}
