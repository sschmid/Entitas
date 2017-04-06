using System;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static DependencyResolver codeGeneratorDependencyResolver {
            get {
                var config = new CodeGeneratorConfig(Preferences.LoadConfig());
                var resolver = new DependencyResolver(AppDomain.CurrentDomain, config.assemblyBasePaths);
                foreach(var path in config.codeGeneratorAssemblyPaths) {
                    resolver.Load(path);
                }

                return resolver;
            }
        }

        public static DependencyResolver dependencyResolver {
            get {
                var config = new CodeGeneratorConfig(Preferences.LoadConfig());
                var resolver = new DependencyResolver(AppDomain.CurrentDomain, config.assemblyBasePaths);
                foreach(var path in config.assemblyPaths) {
                    resolver.Load(path);
                }

                return resolver;
            }
        }

        public static CodeGenerator CodeGeneratorFromConfig(string configPath) {
            Preferences.configPath = configPath;
            var config = new CodeGeneratorConfig(Preferences.LoadConfig());
            var types = LoadTypesFromCodeGeneratorAssemblies();

            return new CodeGenerator(
                GetEnabledInstances<ICodeGeneratorDataProvider>(types, config.dataProviders),
                GetEnabledInstances<ICodeGenerator>(types, config.codeGenerators),
                GetEnabledInstances<ICodeGenFilePostProcessor>(types, config.postProcessors)
            );
        }

        public static Type[] LoadTypesFromAssemblies() {
            return dependencyResolver.GetTypes();
        }

        public static Type[] LoadTypesFromCodeGeneratorAssemblies() {
            return codeGeneratorDependencyResolver.GetTypes();
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
