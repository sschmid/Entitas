using System;
using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class CodeGenerator {
        public const string componentSuffix = "Component";

        public static void Generate(Type[] types, string[] poolNames, string dir) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            CleanDir(dir);
            var components = GetComponents(types);
            generateIndicesLookup(dir, components);
            generateComponentExtensions(dir, components);
            generatePoolAttributes(dir, poolNames);
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
            var files = new IndicesLookupGenerator().Generate(components);
            writeFiles(dir, files);
        }

        static void generateComponentExtensions(string dir, Type[] components) {
            var files = new ComponentExtensionsGenerator().Generate(components);
            writeFiles(dir, files);
        }

        static void generatePoolAttributes(string dir, string[] poolNames) {
            var files = new PoolAttributeGenerator().Generate(poolNames);
            writeFiles(dir, files);
        }

        static void writeFiles(string dir, CodeGenFile[] files) {
            foreach (var file in files) {
                File.WriteAllText(file.fileName + ".cs", file.fileContent);
            }
        }
    }

    public static class EntitasCodeGeneratorExtensions {
        public static string RemoveComponentSuffix(this Type type) {
            const string componentSuffix = CodeGenerator.componentSuffix;
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
