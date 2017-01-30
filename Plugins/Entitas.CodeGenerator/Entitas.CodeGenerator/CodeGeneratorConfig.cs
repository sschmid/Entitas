using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class CodeGeneratorConfig {

        public const string GENERATED_FOLDER_PATH_KEY = "Entitas.CodeGenerator.GeneratedFolderPath";
        const string DEFAULT_GENERATED_FOLDER_PATH = "Assets/Generated/";
        public string generatedFolderPath { 
            get { return _config.GetValueOrDefault(GENERATED_FOLDER_PATH_KEY, DEFAULT_GENERATED_FOLDER_PATH); }
            set { _config[GENERATED_FOLDER_PATH_KEY] = value; }
        }

        public const string CONTEXS_KEY = "Entitas.CodeGenerator.Contexts";
        public string[] contexts {
            get { return separateValues(_config.GetValueOrDefault(CONTEXS_KEY, string.Empty)); }
            set { _config[CONTEXS_KEY] = joinValues(value); }
        }

        public const string ENABLED_CODE_GENERATORS_KEY = "Entitas.CodeGenerator.EnabledCodeGenerators";
        public string[] enabledCodeGenerators {
            get {  return separateValues(_config.GetValueOrDefault(ENABLED_CODE_GENERATORS_KEY, _defaultEnabledCodeGenerators)); }
            set { _config[ENABLED_CODE_GENERATORS_KEY] = joinValues(value); }
        }

        readonly string _defaultEnabledCodeGenerators;

        readonly EntitasPreferencesConfig _config;

        public CodeGeneratorConfig(EntitasPreferencesConfig config, string[] codeGenerators) {
            _config = config;
            _defaultEnabledCodeGenerators = joinValues(codeGenerators);

            // Assigning will apply default values to missing keys
            generatedFolderPath = generatedFolderPath;
            contexts = contexts;
            enabledCodeGenerators = enabledCodeGenerators;
        }

        static string joinValues(string[] values) {
            return string.Join(",",
                                values
                                .Where(value => !string.IsNullOrEmpty(value))
                                .ToArray()
                              ).Replace(" ", string.Empty);
        }

        static string[] separateValues(string values) {
            return values
                        .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(value => value.Trim())
                        .ToArray();
        }

        public override string ToString() {
            return _config.ToString();
        }
    }
}
