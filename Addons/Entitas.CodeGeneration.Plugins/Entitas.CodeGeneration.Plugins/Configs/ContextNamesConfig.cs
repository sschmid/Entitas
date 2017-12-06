using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextNamesConfig : AbstractConfigurableConfig {

        const string CONTEXTS_KEY = "Entitas.CodeGeneration.Plugins.Contexts";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { CONTEXTS_KEY, "Game, Input" }
                };
            }
        }

        public string[] contextNames {
            get { return _preferences[CONTEXTS_KEY].ArrayFromCSV(); }
            set { _preferences[CONTEXTS_KEY] = value.ToCSV(); }
        }
    }
}
