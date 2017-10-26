using System.IO;

namespace Entitas.Utils {

    public class Preferences {

        public const string PATH = "Entitas.properties";
        public const string USER_PATH = "User.properties";

        public Properties properties { get { return _properties; } }

        Properties _properties;
        Properties _userProperties;

        public static bool HasProperties() {
            return File.Exists(PATH);
        }

        public Preferences() {
            _properties = new Properties(string.Empty);
            _userProperties = new Properties(string.Empty);
        }

        public Preferences(Properties properties, Properties userProperties = null) {
            _properties = properties;
            _userProperties = userProperties ?? new Properties(string.Empty);
        }

        public void Load() {
            _properties = new Properties(File.ReadAllText(PATH));

            if (File.Exists(USER_PATH)) {
                _userProperties = new Properties(File.ReadAllText(USER_PATH));
            }
        }

        public void Save() {
            File.WriteAllText(PATH, _properties.ToString());
        }

        public string this[string key] {
            get { return new Properties(_properties.ToDictionary().Merge(_userProperties.ToDictionary()))[key]; }
            set {
                if (value != this[key]) {
                    _properties[key] = value;
                }
            }
        }

        public override string ToString() {
            return _properties + (_userProperties.count != 0 ? ("\n" + _userProperties) : string.Empty);
        }
    }
}
