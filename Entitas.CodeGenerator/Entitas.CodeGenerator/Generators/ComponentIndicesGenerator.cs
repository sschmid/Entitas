using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class ComponentIndicesGenerator : IPoolCodeGenerator, IComponentCodeGenerator {

        // Important: This method should be called before Generate(componentInfos)
        // This will generate empty lookups for all pools.
        public CodeGenFile[] Generate(string[] poolNames) {
            var emptyInfos = new ComponentInfo[0];
            if (poolNames.Length == 0) {
                poolNames = new [] { string.Empty };
            }
            var generatorName = typeof(ComponentIndicesGenerator).FullName;
            return poolNames
                .Select(poolName => poolName + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG)
                .Select(lookupTag => new CodeGenFile {
                    fileName = lookupTag,
                    fileContent = generateIndicesLookup(lookupTag, emptyInfos).ToUnixLineEndings(),
                    generatorName = generatorName
                }).ToArray();
        }

        // Important: This method should be called after Generate(poolNames)
        // This will overwrite the empty lookups with the actual content.
        public CodeGenFile[] Generate(ComponentInfo[] componentInfos) {
            var orderedComponentInfos = componentInfos.OrderBy(info => info.typeName).ToArray();
            var lookupTagToComponentInfosMap = getLookupTagToComponentInfosMap(orderedComponentInfos);
            var generatorName = typeof(ComponentIndicesGenerator).FullName;
            return lookupTagToComponentInfosMap
                .Select(kv => new CodeGenFile {
                    fileName = kv.Key,
                    fileContent = generateIndicesLookup(kv.Key, kv.Value.ToArray()).ToUnixLineEndings(),
                    generatorName = generatorName
                }).ToArray();
        }

        static Dictionary<string, ComponentInfo[]> getLookupTagToComponentInfosMap(ComponentInfo[] componentInfos) {
            var currentIndex = 0;
            var orderedComponentInfoToLookupTagsMap = componentInfos
                .Where(info => info.generateIndex)
                .ToDictionary(info => info, info => info.ComponentLookupTags())
                .OrderByDescending(kv => kv.Value.Length);

            return orderedComponentInfoToLookupTagsMap
                .Aggregate(new Dictionary<string, ComponentInfo[]>(), (map, kv) => {
                    var info = kv.Key;
                    var lookupTags = kv.Value;
                    var incrementIndex = false;
                    foreach (var lookupTag in lookupTags) {
                        if (!map.ContainsKey(lookupTag)) {
                            map.Add(lookupTag, new ComponentInfo[componentInfos.Length]);
                        }

                        var infos = map[lookupTag];
                        if (lookupTags.Length == 1) {
                            // Component has only one lookupTag. Insert at next free slot.
                            for (int i = 0; i < infos.Length; i++) {
                                if (infos[i] == null) {
                                    infos[i] = info;
                                    break;
                                }
                            }
                        } else {
                            // Component has multiple lookupTags. Set at current index in all lookups.
                            infos[currentIndex] = info;
                            incrementIndex = true;
                        }
                    }
                    if (incrementIndex) {
                        currentIndex++;
                    }
                    return map;
                });
        }

        static string generateIndicesLookup(string lookupTag, ComponentInfo[] componentInfos) {
            return addClassHeader(lookupTag)
                    + addIndices(componentInfos)
                    + addComponentNames(componentInfos)
                    + addComponentTypes(componentInfos)
                    + addCloseClass();
        }

        static string addClassHeader(string lookupTag) {
            return string.Format("public static class {0} {{\n", lookupTag);
        }

        static string addIndices(ComponentInfo[] componentInfos) {
            const string fieldFormat = "    public const int {0} = {1};\n";
            const string totalFormat = "    public const int TotalComponents = {0};";
            var code = string.Empty;
            for (int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if (info != null) {
                    code += string.Format(fieldFormat, info.typeName.RemoveComponentSuffix(), i);
                }
            }

            var totalComponents = string.Format(totalFormat, componentInfos.Count(info => info != null));
            return code + "\n" + totalComponents;
        }

        static string addComponentNames(ComponentInfo[] componentInfos) {
            const string format = "        \"{1}\",\n";
            var code = string.Empty;
            for (int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if (info != null) {
                    code += string.Format(format, i, info.typeName.RemoveComponentSuffix());
                }
            }
            if (code.EndsWith(",\n")) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly string[] componentNames = {{
{0}    }};", code);
        }

        static string addComponentTypes(ComponentInfo[] componentInfos) {
            const string format = "        typeof({1}),\n";
            var code = string.Empty;
            for (int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if (info != null) {
                    code += string.Format(format, i, info.fullTypeName);
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
    }
}