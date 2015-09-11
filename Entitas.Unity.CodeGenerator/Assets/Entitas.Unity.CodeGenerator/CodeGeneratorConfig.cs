using System;
using System.Linq;

namespace Entitas.Unity.CodeGenerator {
    public class CodeGeneratorConfig {
        public string generatedFolderPath { 
            get { return _config.GetValueOrDefault(generatedFolderPathKey, defaultGeneratedFolderPath); }
            set { _config[generatedFolderPathKey] = value; }
        }

        public string[] pools {
            get { 
                return _config.GetValueOrDefault(poolsKey, string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(poolName => poolName.Trim())
                    .ToArray();
            }
            set { _config[poolsKey] = string.Join(",", value.Where(pool => !string.IsNullOrEmpty(pool)).ToArray()).Replace(" ", string.Empty); }
        }

        public string[] enabledCodeGenerators {
            get { 
                return _config.GetValueOrDefault(enabledCodeGeneratorsKey, _defaultEnabledCodeGenerators)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(generator => generator.Trim())
                    .ToArray();
            }
            set { _config[enabledCodeGeneratorsKey] = joinCodeGenerators(value); }
        }

        const string generatedFolderPathKey = "Entitas.Unity.CodeGenerator.GeneratedFolderPath";
        const string poolsKey = "Entitas.Unity.CodeGenerator.Pools";
        const string enabledCodeGeneratorsKey = "Entitas.Unity.CodeGenerator.EnabledCodeGenerators";

        const string defaultGeneratedFolderPath = "Assets/Generated/";
        string _defaultEnabledCodeGenerators;

        readonly EntitasPreferencesConfig _config;

        public CodeGeneratorConfig(EntitasPreferencesConfig config, string[] codeGenerators) {
            _config = config;
            _defaultEnabledCodeGenerators = joinCodeGenerators(codeGenerators);
            generatedFolderPath = generatedFolderPath;
            pools = pools;
            enabledCodeGenerators = enabledCodeGenerators;
        }

        static string joinCodeGenerators(string[] codeGenerators) {
            return string.Join(
                                ",",
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

