using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class EntitasCodeGenerator {
        public static string generatedFolder = "Generated/";
        public const string componentSuffix = "Component";

        public static void Generate(Assembly assembly) {
            if (!Directory.Exists(generatedFolder)) {
                Directory.CreateDirectory(generatedFolder);
            }

            CleanGeneratedFolder();
            var components = GetComponents(assembly.GetTypes());
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

        public static Type[]GetComponents(Type[] types) {
            List<Type> components = new List<Type>();
            foreach (var type in types) {
                if (type.GetInterfaces().Contains(typeof(IComponent))) {
                    components.Add(type);
                }
            }

            return components.ToArray();
        }

        static void generateIndicesLookup(Type[] components) {
            var generator = new IndicesLookupGenerator();
            var lookups = generator.GenerateIndicesLookup(components);
            writeFiles(lookups);
        }

        static void generateComponentExtensions(Type[] components) {
            var generator = new ComponentExtensionsGenerator();
            var extensions = generator.GenerateComponentExtensions(components);
            writeFiles(extensions);
        }

        static void writeFiles(Dictionary<string, string> files) {
            foreach (var entry in files) {
                var filePath = generatedFolder + entry.Key + ".cs";
                var code = entry.Value;
                write(filePath, code);
            }
        }

        readonly static object _writeLock = new object();

        static void write(string path, string text) {
            lock (_writeLock) {
                using (StreamWriter writer = new StreamWriter(path, false)) {
                    writer.WriteLine(text);
                }
            }
        }
    }

    public static class EntitasCodeGeneratorExtensions {
        public static string RemoveComponentSuffix(this Type type) {
            const string componentSuffix = EntitasCodeGenerator.componentSuffix;
            if (type.Name.EndsWith(componentSuffix)) {
                return type.Name.Substring(0, type.Name.Length - componentSuffix.Length);
            }

            return type.Name;
        }
    }
}
