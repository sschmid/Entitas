using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class FixConfig : AbstractCommand {

        public override string trigger { get { return "fix"; } }

        public override void Run(string[] args) {
            if(assertProperties()) {
                var properties = loadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

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
                    fixKeys(null, config.defaultProperties, properties);
                    throw ex;
                }

                fixKeys(configurables, config.defaultProperties, properties);
                fixConfigurableKeys(configurables, properties);
                fixPlugins(types, config, properties);
            }
        }

        static void fixKeys(Dictionary<string, string> configurables, Dictionary<string, string> defaultProperties, Properties properties) {
            var requiredKeys = defaultProperties.Keys.ToArray();
            var requiredKeysWithConfigurables = requiredKeys.ToList().ToArray();

            if(configurables != null) {
                requiredKeysWithConfigurables = requiredKeysWithConfigurables.Concat(configurables.Keys).ToArray();
            }

            foreach(var key in Helper.GetMissingKeys(requiredKeys, properties)) {
                Helper.AddKey("Add missing key", key, defaultProperties[key], properties);
            }

            foreach(var key in Helper.GetUnusedKeys(requiredKeysWithConfigurables, properties)) {
                Helper.RemoveKey("Remove unused key", key, properties);
            }
        }

        static void fixConfigurableKeys(Dictionary<string, string> configurables, Properties properties) {
            foreach(var kv in CodeGeneratorUtil.GetMissingConfigurables(configurables, properties)) {
                Helper.AddKey("Add missing key", kv.Key, kv.Value, properties);
            }
        }

        static void fixPlugins(Type[] types, CodeGeneratorConfig config, Properties properties) {
            var unavailableDataProviders = CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var unavailableCodeGenerators = CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(types, config.codeGenerators);
            var unavailablePostProcessors = CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            var availableDataProviders = CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailable<ICodeGenerator>(types, config.codeGenerators);
            var availablePostProcessors = CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            foreach(var dataProvider in unavailableDataProviders) {
                Helper.RemoveValue("Remove unavailable data provider", dataProvider, config.dataProviders,
                                   values => config.dataProviders = values, properties);
            }

            foreach(var codeGenerator in unavailableCodeGenerators) {
                Helper.RemoveValue("Remove unavailable code generator", codeGenerator, config.codeGenerators,
                                   values => config.codeGenerators = values, properties);
            }

            foreach(var postProcessor in unavailablePostProcessors) {
                Helper.RemoveValue("Remove unavailable post processor", postProcessor, config.postProcessors,
                                   values => config.postProcessors = values, properties);
            }

            foreach(var dataProvider in availableDataProviders) {
                Helper.AddValue("Add available data provider", dataProvider, config.dataProviders,
                                values => config.dataProviders = values, properties);
            }

            foreach(var codeGenerator in availableCodeGenerators) {
                Helper.AddValue("Add available code generator", codeGenerator, config.codeGenerators,
                                values => config.codeGenerators = values, properties);
            }

            foreach(var postProcessor in availablePostProcessors) {
                Helper.AddValue("Add available post processor", postProcessor, config.postProcessors,
                                values => config.postProcessors = values, properties);
            }
        }
    }
}
