using System;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromConfig(string configPath) {
            EntitasPreferences.CONFIG_PATH = configPath;
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig());
            var codeGeneratorAssembly = Assembly.LoadFrom(config.codeGeneratorAssemblyPath);

            return new CodeGenerator(
                getEnabled<ICodeGeneratorDataProvider>(codeGeneratorAssembly, config.dataProviders),
                getEnabled<ICodeGenerator>(codeGeneratorAssembly, config.codeGenerators),
                getEnabled<ICodeGenFilePostProcessor>(codeGeneratorAssembly, config.postProcessors)
            );
        }

        public static Type[] GetOrderedTypes<T>(Assembly assembly) {
            return assembly.GetTypes()
                           .Where(type => type.ImplementsInterface<T>())
                           .OrderBy(type => type.FullName)
                           .ToArray();
        }

        static T[] getEnabled<T>(Assembly assembly, string[] types) {
            return GetOrderedTypes<T>(assembly)
                    .Where(type => types.Contains(type.FullName))
                    .Select(type => (T)Activator.CreateInstance(type))
                    .ToArray();
        }
    }
}
