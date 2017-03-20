using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class CodeGeneratorConfig {

        public const string PROJECT_PATH_KEY = "Entitas.CodeGenerator.Project";
        const string DEFAULT_PROJECT_PATH = "Assembly-CSharp.csproj";
        public string projectPath { 
            get { return _config.GetValueOrDefault(PROJECT_PATH_KEY, DEFAULT_PROJECT_PATH); }
            set { _config[PROJECT_PATH_KEY] = value; }
        }

        public const string ASSEMBLY_PATH_KEY = "Entitas.CodeGenerator.Assembly";
        const string DEFAULT_ASSEMBLY_PATH = "Library/ScriptAssemblies/Assembly-CSharp.dll";
        public string assemblyPath { 
            get { return _config.GetValueOrDefault(ASSEMBLY_PATH_KEY, DEFAULT_ASSEMBLY_PATH); }
            set { _config[ASSEMBLY_PATH_KEY] = value; }
        }

        public const string CODE_GENERATOR_ASSEMBLY_PATH_KEY = "Entitas.CodeGenerator.CodeGeneratorAssembly";
		const string DEFAULT_CODE_GENERATOR_ASSEMBLY_PATH = "Library/ScriptAssemblies/Assembly-CSharp-Editor.dll";
        public string codeGeneratorAssemblyPath { 
            get { return _config.GetValueOrDefault(CODE_GENERATOR_ASSEMBLY_PATH_KEY, DEFAULT_CODE_GENERATOR_ASSEMBLY_PATH); }
            set { _config[CODE_GENERATOR_ASSEMBLY_PATH_KEY] = value; }
        }

        public const string TARGET_DIRECTORY_KEY = "Entitas.CodeGenerator.TargetDirectory";
        const string DEFAULT_TARGET_DIRECTORY = "Assets/Generated/";
        public string targetDirectory { 
            get { return _config.GetValueOrDefault(TARGET_DIRECTORY_KEY, DEFAULT_TARGET_DIRECTORY); }
            set { _config[TARGET_DIRECTORY_KEY] = value; }
        }

        public const string CONTEXS_KEY = "Entitas.CodeGenerator.Contexts";
        const string DEFAULT_CONTETS = "Game,GameState,Input";
        public string[] contexts {
            get { return separateValues(_config.GetValueOrDefault(CONTEXS_KEY, DEFAULT_CONTETS)); }
            set { _config[CONTEXS_KEY] = joinValues(value); }
        }

        public const string DATA_PROVIDERS_KEY = "Entitas.CodeGenerator.DataProviders";
        public string[] dataProviders {
            get {  return separateValues(_config.GetValueOrDefault(DATA_PROVIDERS_KEY, _defaultDataProviders)); }
            set { _config[DATA_PROVIDERS_KEY] = joinValues(value); }
        }

        public const string CODE_GENERATORS_KEY = "Entitas.CodeGenerator.CodeGenerators";
        public string[] codeGenerators {
            get {  return separateValues(_config.GetValueOrDefault(CODE_GENERATORS_KEY, _defaultCodeGenerators)); }
            set { _config[CODE_GENERATORS_KEY] = joinValues(value); }
        }

        public const string POST_PROCESSORS_KEY = "Entitas.CodeGenerator.PostProcessors";
        public string[] postProcessors {
            get {  return separateValues(_config.GetValueOrDefault(POST_PROCESSORS_KEY, _defaultPostProcessors)); }
            set { _config[POST_PROCESSORS_KEY] = joinValues(value); }
        }

        protected readonly EntitasPreferencesConfig _config;

        readonly string _defaultDataProviders;
        readonly string _defaultCodeGenerators;
        readonly string _defaultPostProcessors;

        public CodeGeneratorConfig(EntitasPreferencesConfig config) : this(config, new string[0], new string[0], new string[0]) {
        }

        public CodeGeneratorConfig(EntitasPreferencesConfig config, string[] dataProviders, string[] codeGenerators, string[] postProcessors) {
            _config = config;
            _defaultDataProviders = joinValues(dataProviders);
            _defaultCodeGenerators = joinValues(codeGenerators);
            _defaultPostProcessors = joinValues(postProcessors);

            // Assigning will apply default values to missing keys
            projectPath = projectPath;
            assemblyPath = assemblyPath;
            codeGeneratorAssemblyPath = codeGeneratorAssemblyPath;
            targetDirectory = targetDirectory;
            contexts = contexts;
            this.dataProviders = this.dataProviders;
            this.codeGenerators = this.codeGenerators;
            this.postProcessors = this.postProcessors;
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
