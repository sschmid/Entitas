using System.Collections.Generic;
using System.IO;

namespace Entitas.Utils {

    public class Preferences {

        public static Preferences sharedInstance {
            get {
                if (_sharedInstance == null) {
                    _sharedInstance = new Preferences("Entitas.properties", "User.properties");
                }

                return _sharedInstance;
            }
            set { _sharedInstance = value; }
        }

        static Preferences _sharedInstance;

        public string propertiesPath { get { return _propertiesPath; } }
        public string userPropertiesPath { get { return _userPropertiesPath; } }

        public bool propertiesExist { get { return File.Exists(_propertiesPath); } }
        public bool userPropertiesExist { get { return File.Exists(_userPropertiesPath); } }

        public string[] keys { get { return getMergedProperties().keys; } }

        public Properties properties { get { return _properties; } }
        public Properties userProperties { get { return _userProperties; } }

        readonly string _propertiesPath;
        readonly string _userPropertiesPath;
        Properties _properties;
        Properties _userProperties;

        public Preferences(string propertiesPath, string userPropertiesPath) {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
            Refresh();
        }

        public Preferences(Properties properties, Properties userProperties = null) {
            _properties = properties;
            _userProperties = userProperties ?? new Properties();
        }

        public void Refresh() {
            _properties = loadProperties(_propertiesPath);
            _userProperties = loadProperties(_userPropertiesPath);
        }

        public void Save() {
            File.WriteAllText(_propertiesPath, _properties.ToString());
            File.WriteAllText(_userPropertiesPath, _userProperties.ToString());
        }

        public string this[string key] {
            get { return getMergedProperties()[key]; }
            set {
                if (!_properties.HasKey(key) || value != this[key]) {
                    _properties[key] = value;
                }
            }
        }

        public void AddProperties(Dictionary<string, string> properties, bool overwriteExisting) {
            _properties.AddProperties(properties, overwriteExisting);
        }

        public bool HasKey(string key) {
            return _properties.HasKey(key) || _userProperties.HasKey(key);
        }

        public override string ToString() {
            return getMergedProperties().ToString();
        }

        static Properties loadProperties(string path) {
            return new Properties(File.Exists(path)
                ? File.ReadAllText(path)
                : string.Empty
            );
        }

        Properties getMergedProperties() {
            var mergedProperties = new Properties(_properties.ToDictionary());
            mergedProperties.AddProperties(_userProperties.ToDictionary(), true);
            return mergedProperties;
        }
    }
}
