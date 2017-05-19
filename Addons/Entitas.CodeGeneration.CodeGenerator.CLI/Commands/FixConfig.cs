using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class FixConfig : AbstractCommand {

        public override string trigger { get { return "fix"; } }
        public override string description { get { return "Adds missing or removes unused keys interactively"; } }
        public override string example { get { return "entitas fix"; } }

        public override void Run(string[] args) {
            if (assertProperties()) {
                var properties = loadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                forceAddKeys(config.defaultProperties, properties);

                Type[] types = null;

                try {
                    types = CodeGeneratorUtil.LoadTypesFromPlugins(properties);
                    getConfigurables(types, config);
                } catch (Exception ex) {
                    throw ex;
                }

                var askedRemoveKeys = new HashSet<string>();
                var askedAddKeys = new HashSet<string>();
                while (fix(askedRemoveKeys, askedAddKeys, types, config, properties)) { }
            }
        }

        static Dictionary<string, string> getConfigurables(Type[] types, CodeGeneratorConfig config) {
            return CodeGeneratorUtil.GetConfigurables(
                CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.dataProviders),
                CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.codeGenerators),
                CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.postProcessors)
            );
        }

        static void forceAddKeys(Dictionary<string, string> requiredProperties, Properties properties) {
            var requiredKeys = requiredProperties.Keys.ToArray();
            var missingKeys = Helper.GetMissingKeys(requiredKeys, properties);

            foreach (var key in missingKeys) {
                Helper.ForceAddKey("Will add missing key", key, requiredProperties[key], properties);
            }
        }

        static bool fix(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, Type[] types, CodeGeneratorConfig config, Properties properties) {
            var changed = fixPlugins(askedRemoveKeys, askedAddKeys, types, config, properties);

            forceAddKeys(getConfigurables(types, config), properties);

            var requiredKeys = config.defaultProperties.Merge(getConfigurables(types, config)).Keys.ToArray();
            removeUnusedKeys(askedRemoveKeys, requiredKeys, properties);

            return changed;
        }

        static bool fixPlugins(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, Type[] types, CodeGeneratorConfig config, Properties properties) {
            var changed = false;

            var unavailableDataProviders = CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var unavailableCodeGenerators = CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(types, config.codeGenerators);
            var unavailablePostProcessors = CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            var availableDataProviders = CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailable<ICodeGenerator>(types, config.codeGenerators);
            var availablePostProcessors = CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(types, config.postProcessors);

            foreach (var key in unavailableDataProviders) {
                if (!askedRemoveKeys.Contains(key)) {
                    Helper.RemoveValue("Remove unavailable data provider", key, config.dataProviders,
                                       values => config.dataProviders = values, properties);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in unavailableCodeGenerators) {
                if (!askedRemoveKeys.Contains(key)) {
                    Helper.RemoveValue("Remove unavailable code generator", key, config.codeGenerators,
                                       values => config.codeGenerators = values, properties);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in unavailablePostProcessors) {
                if (!askedRemoveKeys.Contains(key)) {
                    Helper.RemoveValue("Remove unavailable post processor", key, config.postProcessors,
                                       values => config.postProcessors = values, properties);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availableDataProviders) {
                if (!askedAddKeys.Contains(key)) {
                    Helper.AddValue("Add available data provider", key, config.dataProviders,
                                    values => config.dataProviders = values, properties);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availableCodeGenerators) {
                if (!askedAddKeys.Contains(key)) {
                    Helper.AddValue("Add available code generator", key, config.codeGenerators,
                                    values => config.codeGenerators = values, properties);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availablePostProcessors) {
                if (!askedAddKeys.Contains(key)) {
                    Helper.AddValue("Add available post processor", key, config.postProcessors,
                                    values => config.postProcessors = values, properties);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            return changed;
        }

        static void removeUnusedKeys(HashSet<string> askedRemoveKeys, string[] requiredKeys, Properties properties) {
            var unused = Helper.GetUnusedKeys(requiredKeys, properties);
            foreach (var key in unused) {
                if (!askedRemoveKeys.Contains(key)) {
                    Helper.RemoveKey("Remove unused key", key, properties);
                    askedRemoveKeys.Add(key);
                }
            }
        }
    }
}
