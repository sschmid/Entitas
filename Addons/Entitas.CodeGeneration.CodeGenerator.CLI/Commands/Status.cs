using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Status : AbstractCommand {

        public override string trigger { get { return "status"; } }
        public override string description { get { return "Lists available and unavailable plugins"; } }
        public override string example { get { return "entitas status"; } }

        public override void Run(string[] args) {
            if (assertProperties()) {
                var properties = loadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                fabl.Debug(config.ToString());

                Type[] types = null;
                Dictionary<string, string> configurables = null;

                try {
                    types = CodeGeneratorUtil.LoadTypesFromPlugins(properties);
                    configurables = CodeGeneratorUtil.GetConfigurables(
                        CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.dataProviders),
                        CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.codeGenerators),
                        CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.postProcessors)
                    );

                } catch(Exception ex) {
                    printKeyStatus(config.defaultProperties.Keys.ToArray(), properties);
                    throw ex;
                }

                var requiredKeys = config.defaultProperties.Merge(configurables).Keys.ToArray();

                printKeyStatus(requiredKeys, properties);
                printPluginStatus(types, config);
            }
        }

        static void printKeyStatus(string[] requiredKeys, Properties properties) {
            foreach (var key in Helper.GetUnusedKeys(requiredKeys, properties)) {
                fabl.Info("Unused key: " + key);
            }

            foreach (var key in Helper.GetMissingKeys(requiredKeys, properties)) {
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
            foreach (var name in names) {
                fabl.Warn("Unavailable: " + name);
            }
        }

        static void printAvailable(string[] names) {
            foreach (var name in names) {
                fabl.Info("Available: " + name);
            }
        }
    }
}
