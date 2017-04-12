using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public static class Status {

        public static void Run() {
            if(File.Exists(Preferences.configPath)) {
                var properties = Preferences.LoadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                fabl.Debug(config.ToString());

                Type[] types = null;
                KeyValuePair<string, string>[] configurableKeyValuePairs = null;

                try {
                    types = CodeGeneratorUtil.LoadTypesFromCodeGeneratorAssemblies();
                    configurableKeyValuePairs = Helper.GetConfigurableKeyValuePairs(
                        CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.dataProviders),
                        CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.codeGenerators),
                        CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.postProcessors)
                    );
                } catch(Exception ex) {
                    printKeyStatus(null, properties);
                    throw ex;
                }

                printKeyStatus(configurableKeyValuePairs, properties);
                printConfigurableKeyStatus(configurableKeyValuePairs, properties);
                printPluginStatus(types, config);
            } else {
                PrintNoConfig.Run();
            }
        }

        static void printKeyStatus(KeyValuePair<string, string>[] configurableKeyValuePairs, Properties properties) {
            var requiredKeys = new CodeGeneratorConfig().defaultProperties.Keys.ToArray();
            var requiredKeysWithConfigurable = new CodeGeneratorConfig().defaultProperties.Keys.ToArray();

            if(configurableKeyValuePairs != null) {
                requiredKeysWithConfigurable = requiredKeysWithConfigurable.Concat(configurableKeyValuePairs.Select(kv => kv.Key)).ToArray();
            }

            foreach(var key in Helper.GetUnusedKeys(requiredKeysWithConfigurable, properties)) {
                fabl.Info("Unused key: " + key);
            }

            foreach(var key in Helper.GetMissingKeys(requiredKeys, properties)) {
                fabl.Warn("Missing key: " + key);
            }
        }

        static void printConfigurableKeyStatus(KeyValuePair<string, string>[] configurableKeyValuePairs, Properties properties) {
            foreach(var kv in Helper.GetMissingConfigurableKeyValuePairs(configurableKeyValuePairs, properties)) {
                fabl.Warn("Missing key: " + kv.Key);
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
