using System;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromConfig(string configPath) {
            EntitasPreferences.CONFIG_PATH = configPath;
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig());
            var assembly = Assembly.LoadFrom(config.codeGeneratorAssemblyPath);

            return new CodeGenerator(
                GetEnabledInstances<ICodeGeneratorDataProvider>(assembly, config.dataProviders),
                GetEnabledInstances<ICodeGenerator>(assembly, config.codeGenerators),
                GetEnabledInstances<ICodeGenFilePostProcessor>(assembly, config.postProcessors)
            );
        }

        public static T[] GetOrderedInstances<T>(Assembly assembly) where T : ICodeGeneratorInterface {
            return assembly.GetTypes()
                           .Where(type => type.ImplementsInterface<T>())
                           .Where(type => !type.IsAbstract)
                           .Select(type => (T)Activator.CreateInstance(type))
                           .OrderBy(instance => instance.priority)
                           .ThenBy(instance => instance.GetType().ToCompilableString())
                           .ToArray();
        }

        public static string[] GetOrderedTypeNames<T>(Assembly assembly) where T : ICodeGeneratorInterface {
            return GetOrderedInstances<T>(assembly)
                    .Select(instance => instance.GetType().ToCompilableString())
                    .ToArray();
        }

        public static T[] GetEnabledInstances<T>(Assembly assembly, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            return GetOrderedInstances<T>(assembly)
                    .Where(instance => enabledTypeNames.Contains(instance.GetType().ToCompilableString()))
                    .ToArray();
        }

        public static string[] GetAvailable<T>(Assembly assembly, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            return GetOrderedTypeNames<T>(assembly)
                    .Where(typeName => !enabledTypeNames.Contains(typeName))
                    .ToArray();
        }

        public static string[] GetUnavailable<T>(Assembly assembly, string[] enabledTypeNames) where T : ICodeGeneratorInterface {
            var typeNames = GetOrderedTypeNames<T>(assembly);
            return enabledTypeNames
                    .Where(typeName => !typeNames.Contains(typeName))
                    .ToArray();
        }
    }
}
