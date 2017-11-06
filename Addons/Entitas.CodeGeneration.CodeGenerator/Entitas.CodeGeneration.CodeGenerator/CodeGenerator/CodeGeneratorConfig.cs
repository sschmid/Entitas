using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator {

    public class CodeGeneratorConfig : AbstractConfigurableConfig {

        const string SEARCH_PATHS_KEY = "Entitas.CodeGeneration.CodeGenerator.SearchPaths";
        const string PLUGINS_PATHS_KEY = "Entitas.CodeGeneration.CodeGenerator.Plugins";

        const string DATA_PROVIDERS_KEY = "Entitas.CodeGeneration.CodeGenerator.DataProviders";
        const string CODE_GENERATORS_KEY = "Entitas.CodeGeneration.CodeGenerator.CodeGenerators";
        const string POST_PROCESSORS_KEY = "Entitas.CodeGeneration.CodeGenerator.PostProcessors";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { SEARCH_PATHS_KEY, "Assets/Libraries/Entitas, " +
                                        "Assets/Libraries/Entitas/Editor, " +
                                        "CodeGenerator/Plugins/Default, " +
                                        "CodeGenerator/Plugins/Entitas.Roslyn, " +
                                        "/Applications/Unity/Unity.app/Contents/Managed, " +
                                        "/Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity, " +
                                        "/Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem" },

                    { PLUGINS_PATHS_KEY, "Entitas.CodeGeneration.Plugins, " +
                                         "Entitas.VisualDebugging.CodeGeneration.Plugins" },

                    { DATA_PROVIDERS_KEY, string.Empty },
                    { CODE_GENERATORS_KEY, string.Empty },
                    { POST_PROCESSORS_KEY, string.Empty }
                };
            }
        }

        public string[] searchPaths {
            get { return _preferences[SEARCH_PATHS_KEY].ArrayFromCSV(); }
            set { _preferences[SEARCH_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] plugins {
            get { return _preferences[PLUGINS_PATHS_KEY].ArrayFromCSV(); }
            set { _preferences[PLUGINS_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] dataProviders {
            get { return _preferences[DATA_PROVIDERS_KEY].ArrayFromCSV(); }
            set { _preferences[DATA_PROVIDERS_KEY] = value.ToCSV(); }
        }

        public string[] codeGenerators {
            get { return _preferences[CODE_GENERATORS_KEY].ArrayFromCSV(); }
            set { _preferences[CODE_GENERATORS_KEY] = value.ToCSV(); }
        }

        public string[] postProcessors {
            get { return _preferences[POST_PROCESSORS_KEY].ArrayFromCSV(); }
            set { _preferences[POST_PROCESSORS_KEY] = value.ToCSV(); }
        }
    }
}
