using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentIndicesGenerator : IPoolCodeGenerator, IComponentCodeGenerator {

        // Important: This method should be called before Generate(componentInfos)
        // This will generate empty lookups for all pools.
        public CodeGenFile[] Generate(string[] poolNames) {
            var emptyInfos = new ComponentInfo[0];
            var generatorName = GetType().FullName;
            return poolNames
                .Select(poolName => poolName.PoolPrefix() + CodeGenerator.DEFAULT_COMPONENT_LOOKUP_TAG)
                .Select(lookupTag => new CodeGenFile(
                    lookupTag,
                    generateIndicesLookup(lookupTag, emptyInfos),
                    generatorName
                )).ToArray();
        }

        // Important: This method should be called after Generate(poolNames)
        // This will overwrite the empty lookups with the actual content.
        public CodeGenFile[] Generate(ComponentInfo[] componentInfos) {
            var orderedComponentInfos = componentInfos.OrderBy(info => info.typeName).ToArray();
            var lookupTagToComponentInfosMap = getLookupTagToComponentInfosMap(orderedComponentInfos);
            var generatorName = GetType().FullName;
            return lookupTagToComponentInfosMap
                .Select(kv => new CodeGenFile(
                    kv.Key,
                    generateIndicesLookup(kv.Key, kv.Value.ToArray()),
                    generatorName
                )).ToArray();
        }

        static Dictionary<string, ComponentInfo[]> getLookupTagToComponentInfosMap(ComponentInfo[] componentInfos) {
            var currentIndex = 0;

            // order componentInfos by pool count
            var orderedComponentInfoToLookupTagsMap = componentInfos
                .Where(info => info.generateIndex)
                .ToDictionary(info => info, info => info.ComponentLookupTags())
                .OrderByDescending(kv => kv.Value.Length);

            var lookupTagToComponentInfosMap = orderedComponentInfoToLookupTagsMap
                .Aggregate(new Dictionary<string, ComponentInfo[]>(), (map, kv) => {
                    var info = kv.Key;
                    var lookupTags = kv.Value;
                    var componentIsAssignedToMultiplePools = lookupTags.Length > 1;
                    var incrementIndex = false;
                    foreach(var lookupTag in lookupTags) {
                        if(!map.ContainsKey(lookupTag)) {
                            map.Add(lookupTag, new ComponentInfo[componentInfos.Length]);
                        }

                        var infos = map[lookupTag];
                        if(componentIsAssignedToMultiplePools) {
                            // Component has multiple lookupTags. Set at current index in all lookups.
                            infos[currentIndex] = info;
                            incrementIndex = true;
                        } else {
                            // Component has only one lookupTag. Insert at next free slot.
                            for(int i = 0; i < infos.Length; i++) {
                                if(infos[i] == null) {
                                    infos[i] = info;
                                    break;
                                }
                            }
                        }
                    }
                    if(incrementIndex) {
                        currentIndex++;
                    }

                    return map;
                });


            foreach(var key in lookupTagToComponentInfosMap.Keys.ToArray()) {
                var infoList = lookupTagToComponentInfosMap[key].ToList();
                while(infoList.Count != 0) {
                    var last = infoList.Count - 1;
                    if(infoList[last] == null) {
                        infoList.RemoveAt(last);
                    } else {
                        break;
                    }
                }
                lookupTagToComponentInfosMap[key] = infoList.ToArray();
            }

            return lookupTagToComponentInfosMap;
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
            for(int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if(info != null) {
                    code += string.Format(fieldFormat, info.typeName.RemoveComponentSuffix(), i);
                }
            }

            var totalComponents = string.Format(totalFormat, componentInfos.Length);
            return code + "\n" + totalComponents;
        }

        static string addComponentNames(ComponentInfo[] componentInfos) {
            const string format = "        \"{1}\",\n";
            const string nullFormat = "        null,\n";
            var code = string.Empty;
            for(int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if(info != null) {
                    code += string.Format(format, i, info.typeName.RemoveComponentSuffix());
                } else {
                    code += nullFormat;
                }
            }
            if(code.EndsWith(",\n")) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly string[] componentNames = {{
{0}    }};", code);
        }

        static string addComponentTypes(ComponentInfo[] componentInfos) {
            const string format = "        typeof({1}),\n";
            const string nullFormat = "        null,\n";
            var code = string.Empty;
            for(int i = 0; i < componentInfos.Length; i++) {
                var info = componentInfos[i];
                if(info != null) {
                    code += string.Format(format, i, info.fullTypeName);
                } else {
                    code += nullFormat;
                }
            }
            if(code.EndsWith(",\n")) {
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