using System;
using System.Linq;

namespace Entitas.Unity.CodeGenerator {
    public class CodeGeneratorConfig {
        public string generatedFolderPath { 
            get { return _config.GetValueOrDefault(GENERATED_FOLDER_PATH_KEY, DEFAULT_GENERATED_FOLDER_PATH); }
            set { _config[GENERATED_FOLDER_PATH_KEY] = value; }
        }

        public string[] pools {
            get { 
                return _config.GetValueOrDefault(POOLS_KEY, string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(poolName => poolName.Trim())
                    .ToArray();
            }
            set { _config[POOLS_KEY] = string.Join(",", value.Where(pool => !string.IsNullOrEmpty(pool)).ToArray()).Replace(" ", string.Empty); }
        }

        public string[] enabledCodeGenerators {
            get { 
                return _config.GetValueOrDefault(ENABLED_CODE_GENERATORS_KEY, _defaultEnabledCodeGenerators)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(generator => generator.Trim())
                    .ToArray();
            }
            set { _config[ENABLED_CODE_GENERATORS_KEY] = joinCodeGenerators(value); }
        }

        const string GENERATED_FOLDER_PATH_KEY = "Entitas.Unity.CodeGenerator.GeneratedFolderPath";
        const string POOLS_KEY = "Entitas.Unity.CodeGenerator.Pools";
        const string ENABLED_CODE_GENERATORS_KEY = "Entitas.Unity.CodeGenerator.EnabledCodeGenerators";

        const string DEFAULT_GENERATED_FOLDER_PATH = "Assets/Generated/";
        readonly string _defaultEnabledCodeGenerators;

        readonly EntitasPreferencesConfig _config;

        public CodeGeneratorConfig(EntitasPreferencesConfig config, string[] codeGenerators) {
            _config = config;
            _defaultEnabledCodeGenerators = joinCodeGenerators(codeGenerators);
            generatedFolderPath = generatedFolderPath;
            pools = pools;
            enabledCodeGenerators = enabledCodeGenerators;
        }

        static string joinCodeGenerators(string[] codeGenerators) {
            return string.Join(",",
                                codeGenerators
                                    .Where(generator => !string.IsNullOrEmpty(generator))
                                    .ToArray()
                            ).Replace(" ", string.Empty);
        }

        public override string ToString() {
            return _config.ToString();
        }
    }
}

