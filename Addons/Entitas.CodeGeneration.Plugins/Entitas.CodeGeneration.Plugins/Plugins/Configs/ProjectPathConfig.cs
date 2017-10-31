using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ProjectPathConfig : AbstractConfigurableConfig {

        const string PROJECT_PATH_KEY = "Entitas.CodeGeneration.Plugins.ProjectPath";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { PROJECT_PATH_KEY, "Assembly-CSharp.csproj" }
                };
            }
        }

        public string projectPath {
            get { return _preferences[PROJECT_PATH_KEY]; }
        }
    }
}
