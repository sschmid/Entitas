using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration {

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
