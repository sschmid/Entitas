namespace Entitas.Unity {
    public class EntitasPreferencesConfig {

        readonly Properties _properties;

        public EntitasPreferencesConfig(string config) {
            _properties = new Properties(config);
        }

        public string this[string key] {
            get { return _properties[key]; }
            set { _properties[key] = value; }
        }

        public override string ToString() {
            return _properties.ToString();
        }

        public string GetValueOrDefault(string key, string defaultValue) {
            key = key.Trim();
            if (_properties.HasKey(key)) {
                return _properties[key];
            }

            _properties[key] = defaultValue;
            return _properties[key];
        }
    }
}
