using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class CodeGenerator {
        public const string DEFAULT_INDICES_LOOKUP_TAG = "ComponentIds";

        public static void Generate(Type[] types, string[] poolNames, string dir, ICodeGenerator[] codeGenerators) {
            dir = GetSafeDir(dir);
            CleanDir(dir);
            
            foreach (var generator in codeGenerators.OfType<IPoolCodeGenerator>()) {
                writeFiles(dir, generator.Generate(poolNames));
            }

            var components = GetComponents(types);
            foreach (var generator in codeGenerators.OfType<IComponentCodeGenerator>()) {
                writeFiles(dir, generator.Generate(components));
            }

            var systems = GetSystems(types);
            foreach (var generator in codeGenerators.OfType<ISystemCodeGenerator>()) {
                writeFiles(dir, generator.Generate(systems));
            }
        }

        public static string GetSafeDir(string dir) {
            if (!dir.EndsWith("/", StringComparison.Ordinal)) {
                dir += "/";
            }
            if (!dir.EndsWith("Generated/", StringComparison.Ordinal)) {
                dir += "Generated/";
            }
            return dir;
        }

        public static void CleanDir(string dir) {
            dir = GetSafeDir(dir);
            if (Directory.Exists(dir)) {
                var files = new DirectoryInfo(dir).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach (var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(dir);
            }
        }

        public static Type[] GetComponents(Type[] types) {
            return types
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .ToArray();
        }

        public static Type[] GetSystems(Type[] types) {
            return types
                .Where(type => !type.IsInterface
                    && type != typeof(ReactiveSystem)
                    && type != typeof(Systems)
                    && type.GetInterfaces().Contains(typeof(ISystem)))
                .ToArray();
        }

        static void writeFiles(string dir, CodeGenFile[] files) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            foreach (var file in files) {
                File.WriteAllText(dir + file.fileName + ".cs", file.fileContent.Replace("\n", Environment.NewLine));
            }
        }
    }

    public static class CodeGeneratorExtensions {
        public static string[] PoolNames(this Type type) {
            return Attribute.GetCustomAttributes(type)
                .Aggregate(new List<string>(), (poolNames, attr) => {
                    var poolAttribute = attr as PoolAttribute;
                    if (poolAttribute != null) {
                        poolNames.Add(poolAttribute.tag);
                    }

                    return poolNames;
                })
                .OrderBy(poolName => poolName)
                .ToArray();
        }

        public static string[] IndicesLookupTags(this Type type) {
            var poolNames = type.PoolNames();
            if (poolNames.Length == 0) {
                return new [] { CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG };
            }

            return poolNames
                .Select(poolName => poolName + CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG)
                .ToArray();
        }

        public static string CustomPrefix(this Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<CustomPrefixAttribute>()
                .SingleOrDefault();

            return attr != null ? attr.prefix : "is" ;
        }

        public static string UppercaseFirst(this string str) {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str) {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static string ToUnixLineEndings(this string str) {
            return str.Replace(Environment.NewLine, "\n");
        }
    }
}
