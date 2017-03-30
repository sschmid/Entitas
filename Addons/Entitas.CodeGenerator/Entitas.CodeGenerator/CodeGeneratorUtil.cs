using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static DependencyResolver codeGeneratorDependencyResolver {
            get {
                if(_codeGeneratorDependencyResolver == null) {
                    var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(EntitasPreferences.GetConfigPath()));
                    _codeGeneratorDependencyResolver = new DependencyResolver(AppDomain.CurrentDomain, config.assemblyBasePaths);
                    foreach(var path in config.codeGeneratorAssemblyPaths) {
                        _codeGeneratorDependencyResolver.Load(path);
                    }
                }

                return _codeGeneratorDependencyResolver;
            }
        }

        public static DependencyResolver dependencyResolver {
            get {
                if(_dependencyResolver == null) {
                    var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(EntitasPreferences.GetConfigPath()));
                    _dependencyResolver = new DependencyResolver(AppDomain.CurrentDomain, config.assemblyBasePaths);
                    foreach(var path in config.assemblyPaths) {
                        _dependencyResolver.Load(path);
                    }
                }

                return _dependencyResolver;
            }
        }

        static DependencyResolver _dependencyResolver;
        static DependencyResolver _codeGeneratorDependencyResolver;

        public static CodeGenerator CodeGeneratorFromConfig(string configPath) {
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(configPath));
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
