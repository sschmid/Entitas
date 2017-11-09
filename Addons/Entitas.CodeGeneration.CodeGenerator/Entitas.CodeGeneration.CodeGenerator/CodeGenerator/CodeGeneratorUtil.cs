using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromPreferences(Preferences preferences) {
            var types = LoadTypesFromPlugins(preferences);

            var config = new CodeGeneratorConfig();
            config.Configure(preferences);

            var dataProviders = GetEnabledInstancesOf<ICodeGeneratorDataProvider>(types, config.dataProviders);
            var codeGenerators = GetEnabledInstancesOf<ICodeGenerator>(types, config.codeGenerators);
            var postProcessors = GetEnabledInstancesOf<ICodeGenFilePostProcessor>(types, config.postProcessors);

            configure(dataProviders, preferences);
            configure(codeGenerators, preferences);
            configure(postProcessors, preferences);

            return new CodeGenerator(dataProviders, codeGenerators, postProcessors);
        }

        static void configure(ICodeGeneratorInterface[] plugins, Preferences preferences) {
            foreach (var plugin in plugins.OfType<IConfigurable>()) {
                plugin.Configure(preferences);
            }
        }

        public static Type[] LoadTypesFromPlugins(Preferences preferences) {
            var config = new CodeGeneratorConfig();
            config.Configure(preferences);
            var resolver = new AssemblyResolver(AppDomain.CurrentDomain, config.searchPaths);
            foreach (var path in config.plugins) {
                resolver.Load(path);
            }

            return resolver.GetTypes();
        }

        public static T[] GetOrderedInstancesOf<T>(Type[] types) where T : ICodeGeneratorInterface {
            return types
                    .Where(type => type.ImplementsInterface<T>())
                    .Where(type => !type.IsAbstract)
                    .Select(type => (T)Activator.CreateInstance(type))
                    .OrderBy(instance => instance.priority)
                    .ThenBy(instance => instance.GetType().ToCompilableString())
                    .ToArray();
        }

        public static string[] GetOrderedTypeNamesOf<T>(Type[] types) where T : ICodeGeneratorInterface {
            return GetOrderedInstancesOf<T>(types)
                    .Select(instance => instance.GetType().ToCompilableString())
                    .ToArray();
        }

        public static T[] GetEnabledInstancesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorInterface {
            return GetOrderedInstancesOf<T>(types)
                    .Where(instance => typeNames.Contains(instance.GetType().ToCompilableString()))
                    .ToArray();
        }

        public static string[] GetAvailableNamesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorInterface {
            return GetOrderedTypeNamesOf<T>(types)
                    .Where(typeName => !typeNames.Contains(typeName))
                    .ToArray();
        }

        public static string[] GetUnavailableNamesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorInterface {
            var orderedTypeNames = GetOrderedTypeNamesOf<T>(types);
            return typeNames
                    .Where(typeName => !orderedTypeNames.Contains(typeName))
                    .ToArray();
        }

        public static Dictionary<string, string> GetDefaultProperties(ICodeGeneratorDataProvider[] dataProviders, ICodeGenerator[] codeGenerators, ICodeGenFilePostProcessor[] postProcessors) {
            return new Dictionary<string, string>()
                .Merge(dataProviders.OfType<IConfigurable>()
                       .Concat(codeGenerators.OfType<IConfigurable>())
                       .Concat(postProcessors.OfType<IConfigurable>())
                       .Select(instance => instance.defaultProperties)
                       .ToArray());
        }
    }
}
