using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class IndicesLookupGenerator : IComponentCodeGenerator, IPoolCodeGenerator {

        public CodeGenFile[] Generate(Type[] components) {
            var sortedComponents = components.OrderBy(type => type.ToString()).ToArray();
            return getLookups(sortedComponents)
                .Aggregate(new List<CodeGenFile>(), (files, lookup) => {
                    files.Add(new CodeGenFile {
                        fileName = lookup.Key,
                        fileContent = generateIndicesLookup(lookup.Key, lookup.Value.ToArray()).ToUnixLineEndings()
                    });
                    return files;
                }).ToArray();
        }

        public CodeGenFile[] Generate(string[] poolNames) {
            var noTypes = new Type[0];
            if (poolNames.Length == 0) {
                poolNames = new [] { string.Empty };
            }
            return poolNames
                .Aggregate(new List<CodeGenFile>(), (files, poolName) => {
                    var lookupTag = poolName + CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG;
                    files.Add(new CodeGenFile {
                        fileName = lookupTag,
                        fileContent = generateIndicesLookup(lookupTag, noTypes).ToUnixLineEndings()
                    });
                    return files;
                }).ToArray();
        }

        static Dictionary<string, Type[]> getLookups(Type[] components) {
            var currentIndex = 0;
            var orderedComponents = components
                .Where(shouldGenerate)
                .Aggregate(new Dictionary<Type, string[]>(), (acc, type) => {
                    acc.Add(type, type.IndicesLookupTags());
                    return acc;
                })
                .OrderByDescending(kv => kv.Value.Length);

            return orderedComponents
                .Aggregate(new Dictionary<string, Type[]>(), (lookups, kv) => {
                    var type = kv.Key;
                    var lookupTags = kv.Value;
                    var incrementIndex = false;
                    foreach (var lookupTag in lookupTags) {
                        if (!lookups.ContainsKey(lookupTag)) {
                            lookups.Add(lookupTag, new Type[components.Length]);
                        }

                        var types = lookups[lookupTag];
                        if (lookupTags.Length == 1) {
                            for (int i = 0; i < types.Length; i++) {
                                if (types[i] == null) {
                                    types[i] = type;
                                    break;
                                }
                            }
                        } else {
                            types[currentIndex] = type;
                            incrementIndex = true;
                        }
                    }
                    if (incrementIndex) {
                        currentIndex++;
                    }
                    return lookups;
                });
        }

        static bool shouldGenerate(Type type) {
            return Attribute.GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .All(attr => attr.generateIndex);
        }

        static string generateIndicesLookup(string tag, Type[] components) {
            return addClassHeader(tag)
                    + addIndices(components)
                    + addComponentNames(components)
                    + addComponentTypes(components)
                    + addCloseClass();
        }

        static string addClassHeader(string lookupTag) {
            return string.Format("public static class {0} {{\n", lookupTag);
        }

        static string addIndices(Type[] components) {
            const string FIELD_FORMAT = "    public const int {0} = {1};\n";
            const string TOTAL_FORMAT = "    public const int TotalComponents = {0};";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                var type = components[i];
                if (type != null) {
                    code += string.Format(FIELD_FORMAT, type.RemoveComponentSuffix(), i);
                }
            }

            return code + "\n" + string.Format(TOTAL_FORMAT, components.Count(type => type != null));
        }

        static string addComponentNames(Type[] components) {
            const string FORMAT = "        \"{1}\",\n";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                var type = components[i];
                if (type != null) {
                    code += string.Format(FORMAT, i, type.RemoveComponentSuffix());
                }
            }
            if (code.EndsWith(",\n")) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly string[] componentNames = {{
{0}    }};", code);
        }

        static string addComponentTypes(Type[] components) {
            const string FORMAT = "        typeof({1}),\n";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                var type = components[i];
                if (type != null) {
                    code += string.Format(FORMAT, i, TypeGenerator.Generate(type));
                }
            }
            if (code.EndsWith(",\n")) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly System.Type[] componentTypes = {{
{0}    }};", code);
        }

        static string addCloseClass() {
            return "\n}";
        }

        static string stripDefaultTag(string tag) {
            return tag.Replace(CodeGenerator.DEFAULT_INDICES_LOOKUP_TAG, string.Empty);
        }
    }
}