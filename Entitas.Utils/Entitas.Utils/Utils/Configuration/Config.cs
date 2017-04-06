namespace Entitas.Utils {

    public class Config {

        readonly Properties _properties;

        public Config(string config) {
            _properties = new Properties(config);
        }

        public string GetValueOrDefault(string key, string defaultValue) {
            key = key.Trim();
            if(!_properties.HasKey(key)) {
                _properties[key] = defaultValue;
            }

            return _properties[key];
        }

        public string this[string key] {
            get { return _properties[key]; }
            set { _properties[key] = value; }
        }

        public override string ToString() {
            return _properties.ToString();
        }
    }
}
