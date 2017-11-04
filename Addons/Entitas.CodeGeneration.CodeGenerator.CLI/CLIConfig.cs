using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class CLIConfig : AbstractConfigurableConfig {

        const string IGNORE_UNUSED_KEYS_KEY = "Entitas.CodeGeneration.CodeGenerator.CLI.Ignore.UnusedKeys";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { IGNORE_UNUSED_KEYS_KEY, "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold, " +
                                              "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath, " +
                                              "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath" }
                };
            }
        }

        public string[] ignoreUnusedKeys {
            get { return _preferences[IGNORE_UNUSED_KEYS_KEY].ArrayFromCSV(); }
            set { _preferences[IGNORE_UNUSED_KEYS_KEY] = value.ToCSV(); }
        }
    }
}
