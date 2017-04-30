using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class TargetDirectoryConfig : AbstractConfigurableConfig {

        const string TARGET_DIRECTORY_KEY = "Entitas.CodeGeneration.Plugins.TargetDirectory";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { TARGET_DIRECTORY_KEY, "Assets/Sources" }
                };
            }
        }

        public string targetDirectory {
            get { return properties[TARGET_DIRECTORY_KEY].ToSafeDirectory(); }
        }
    }
}
