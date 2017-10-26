using System.Collections.Generic;

namespace Entitas.Utils {

    public abstract class AbstractConfigurableConfig : IConfigurable {

        public abstract Dictionary<string, string> defaultProperties { get; }

        public Preferences preferences { get { return _preferences; } }

        Preferences _preferences;

        public virtual void Configure(Preferences preferences) {
            _preferences = preferences;
        }

        public override string ToString() {
            return _preferences.ToString();
        }
    }
}
