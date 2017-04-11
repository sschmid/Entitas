using System.Collections.Generic;

namespace Entitas.Utils {

    public abstract class AbstractConfigurableConfig : IConfigurable {

        public abstract Dictionary<string, string> defaultProperties { get; }

        protected Properties properties {
            get {
                if(_properties == null) {
                    _properties = new Properties(defaultProperties);
                }

                return _properties;
            }
        }

        Properties _properties;

        public virtual void Configure(Properties properties) {
            _properties = properties;
        }
    }
}
