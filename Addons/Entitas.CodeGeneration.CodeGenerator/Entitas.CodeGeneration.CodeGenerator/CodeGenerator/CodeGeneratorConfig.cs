using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator {

    public class CodeGeneratorConfig : AbstractConfigurableConfig {

        const string SEARCH_PATHS_KEY = "CodeGenerator.SearchPaths";
        const string PLUGINS_PATHS_KEY = "CodeGenerator.Plugins";

        const string DATA_PROVIDERS_KEY = "CodeGenerator.DataProviders";
        const string CODE_GENERATORS_KEY = "CodeGenerator.CodeGenerators";
        const string POST_PROCESSORS_KEY = "CodeGenerator.PostProcessors";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { SEARCH_PATHS_KEY, "CodeGenerator/Plugins" },
                    { PLUGINS_PATHS_KEY, string.Empty },
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
