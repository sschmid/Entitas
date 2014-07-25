using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class EntitasCodeGenerator {
        public const string componentSuffix = "Component";
        const string generatedFolder = "Assets/Sources/Generated/";

        public static void GenerateComponentExtensions() {
            if (!Directory.Exists(generatedFolder))
                Directory.CreateDirectory(generatedFolder);

            CleanGeneratedFolder();
            var assembly = Assembly.GetAssembly(typeof(EntitasCodeGenerator));
            var components = getComponentsInAssembly(assembly);
            generateIndicesLookup(components);
            generateComponentExtensions(components);
        }

        public static void CleanGeneratedFolder() {
            if (Directory.Exists(generatedFolder)) {
                FileInfo[] files = new DirectoryInfo(generatedFolder).GetFiles("*.cs");
                foreach (var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            }
        }

        static Type[]getComponentsInAssembly(Assembly assembly) {
            List<Type> components = new List<Type>();
            foreach (var type in assembly.GetTypes()) {
                if (type.Name.EndsWith(componentSuffix) &&
                    !type.FullName.Contains(".")
                    && type.GetInterfaces().Contains(typeof(IComponent))) {
                    components.Add(type);
                }
            }

            return components.ToArray();
        }

        static void generateIndicesLookup(Type[] components) {
            var generator = new IndicesLookupGenerator();
            generator.GenerateIndicesLookup(components, generatedFolder);
        }

        static void generateComponentExtensions(Type[] components) {
            var generator = new ComponentExtensionsGenerator();
            generator.GenerateComponentExtensions(components, generatedFolder);
        }
    }
}