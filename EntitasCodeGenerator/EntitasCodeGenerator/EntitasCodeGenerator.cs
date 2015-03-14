using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class EntitasCodeGenerator {
        public const string componentSuffix = "Component";

        public static void Generate(Assembly assembly, string dir) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            CleanDir(dir);
            var components = GetComponents(assembly.GetTypes());
            generateIndicesLookup(dir, components);
            generateComponentExtensions(dir, components);
        }

        public static void CleanDir(string dir) {
            if (Directory.Exists(dir)) {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.cs");
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
            return types
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .ToArray();
        }

        static void generateIndicesLookup(string dir, Type[] components) {
            var lookups = IndicesLookupGenerator
                .GenerateIndicesLookup(components);
            writeFiles(dir, lookups);
        }

        static void generateComponentExtensions(string dir, Type[] components) {
            var extensions = ComponentExtensionsGenerator
                .GenerateComponentExtensions(components, "GeneratedExtension");
            writeFiles(dir, extensions);
        }

        static void writeFiles(string dir, Dictionary<string, string> files) {
            foreach (var entry in files) {
                var filePath = dir + entry.Key + ".cs";
                var code = entry.Value;
                File.WriteAllText(filePath, code);
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

        public static string PoolName(this Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var customPool = attr as PoolAttribute;
                if (customPool != null) {
                    return customPool.tag;
                }
            }

            return string.Empty;
        }

        public static string IndicesLookupTag(this Type type) {
            return type.PoolName() + "ComponentIds";
        }

        public static string UppercaseFirst(this string str) {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str) {
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
