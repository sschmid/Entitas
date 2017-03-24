using System;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromConfig(string configPath) {
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(configPath));
            var types = GetTypesInAllAssemblies(config);

            return new CodeGenerator(
                GetEnabledInstances<ICodeGeneratorDataProvider>(types, config.dataProviders),
                GetEnabledInstances<ICodeGenerator>(types, config.codeGenerators),
                GetEnabledInstances<ICodeGenFilePostProcessor>(types, config.postProcessors)
            );
        }

        public static Type[] GetTypesInAllAssemblies(string configPath) {
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(configPath));
            return GetTypesInAllAssemblies(config);
        }

        public static Type[] GetTypesInAllAssemblies(CodeGeneratorConfig config) {
            return config.codeGeneratorAssemblyPaths
                         .Select(path => {

                             System.Console.WriteLine("### Load " + path);

                             return Assembly.LoadFrom(path);
                         })
                         .SelectMany(assembly => assembly.GetTypes())
                         .ToArray();
        }

        public static T[] GetOrderedInstances<T>(Type[] types) where T : ICodeGeneratorInterface {
            return types
                    .Where(type => type.ImplementsInterface<T>())
                    .Where(type => !type.IsAbstract)
                    .Select(type => (T)Activator.CreateInstance(type))
                    .OrderBy(instance => instance.priority)
                    .ThenBy(instance => instance.GetType().ToCompilableString())
                    .ToArray();
        }

        public static string[] GetOrderedTypeNames<T>(Type[] types) where T : ICodeGeneratorInterface {
            return GetOrderedInstances<T>(types)
                    .Select(instance => instance.GetType().ToCompilableString())
                    .ToArray();
        }

        public static T[] GetEnabledInstances<T>(Type[] types, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            return GetOrderedInstances<T>(types)
                    .Where(instance => enabledTypeNames.Contains(instance.GetType().ToCompilableString()))
                    .ToArray();
        }

        public static string[] GetAvailable<T>(Type[] types, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            return GetOrderedTypeNames<T>(types)
                    .Where(typeName => !enabledTypeNames.Contains(typeName))
                    .ToArray();
        }

        public static string[] GetUnavailable<T>(Type[] types, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            var typeNames = GetOrderedTypeNames<T>(types);
            return enabledTypeNames
                    .Where(typeName => !typeNames.Contains(typeName))
                    .ToArray();
        }
    }
}
