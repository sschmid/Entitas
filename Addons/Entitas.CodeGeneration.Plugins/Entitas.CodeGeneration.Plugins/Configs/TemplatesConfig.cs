using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class TemplatesConfig : AbstractConfigurableConfig {

        const string TEMPLATES_KEY = "Entitas.CodeGeneration.Plugins.Templates";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { TEMPLATES_KEY, "Plugins/Entitas/Templates" }
                };
            }
        }

        public string[] templates {
            get { return _preferences[TEMPLATES_KEY].ArrayFromCSV(); }
            set { _preferences[TEMPLATES_KEY] = value.ToCSV(); }
        }
    }
}
