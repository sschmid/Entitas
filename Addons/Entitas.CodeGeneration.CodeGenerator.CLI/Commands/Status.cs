using Entitas.Utils;
using System.IO;
using System;
using Fabl;
using System.Linq;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class Status {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                var properties = Preferences.LoadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                fabl.Debug(config.ToString());

                Type[] types = null;
                string[] configurableKeys = null;

                try {
                    types = CodeGeneratorUtil.LoadTypesFromCodeGeneratorAssemblies();
                    configurableKeys = Helper.GetConfigurableKeys(
                        CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.dataProviders),
                        CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.codeGenerators),
                        CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.postProcessors)
                    );
                } catch(Exception ex) {
                    printKeyStatus(null, properties);
                    throw ex;
                }

                printKeyStatus(configurableKeys, properties);
                printConfigurableKeyStatus(configurableKeys, properties);
                printPluginStatus(types, config);
            } else {
                PrintNoConfig.Run();
            }
        }

        static void printKeyStatus(string[] configurableKeys, Properties properties) {
            var requiredKeys = new CodeGeneratorConfig().defaultProperties.Keys.ToArray();
            var requiredKeysWithConfigurable = new CodeGeneratorConfig().defaultProperties.Keys.ToArray();

            if(configurableKeys != null) {
                requiredKeysWithConfigurable = requiredKeysWithConfigurable.Concat(configurableKeys).ToArray();
            }

            foreach(var key in Helper.GetUnusedKeys(requiredKeysWithConfigurable, properties)) {
                fabl.Info("Unused key: " + key);
            }

            foreach(var key in Helper.GetMissingKeys(requiredKeys, properties)) {
                fabl.Warn("Missing key: " + key);
            }
        }

        static void printConfigurableKeyStatus(string[] configurableKeys, Properties properties) {
            foreach(var key in Helper.GetMissingConfigurableKeys(configurableKeys, properties)) {
                fabl.Warn("Missing key: " + key);
            }
        }

        static void printPluginStatus(Type[] types, CodeGeneratorConfig config) {
            var unavailableDataProviders = CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var unavailableCodeGenerators = CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(types, config.codeGenerators);
            var unavailablePostProcessors = CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            var availableDataProviders = CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailable<ICodeGenerator>(types, config.codeGenerators);
            var availablePostProcessors = CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            printUnavailable(unavailableDataProviders);
            printUnavailable(unavailableCodeGenerators);
            printUnavailable(unavailablePostProcessors);

            printAvailable(availableDataProviders);
            printAvailable(availableCodeGenerators);
            printAvailable(availablePostProcessors);
        }

        static void printUnavailable(string[] names) {
            foreach(var name in names) {
                fabl.Warn("Unavailable: " + name);
            }
        }

        static void printAvailable(string[] names) {
            foreach(var name in names) {
                fabl.Info("Available: " + name);
            }
        }
    }
}
