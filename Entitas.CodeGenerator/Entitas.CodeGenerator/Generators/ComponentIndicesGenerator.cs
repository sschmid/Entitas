using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentIndicesGenerator : ICodeGenerator {

        public const string COMPONENT_LOOKUP = "ComponentIds";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var files = new List<CodeGenFile>();
            files.AddRange(generateEmptyLookupsForContexts(data));
            files.AddRange(generateLookupsForContexts(data));
            return files.ToArray();
        }

        CodeGenFile[] generateEmptyLookupsForContexts(CodeGeneratorData[] data) {
            var generatorName = GetType().FullName;
            var emptyData = new CodeGeneratorData[0];
            return data
                .Where(d => d.ContainsKey(ComponentDataProvider.COMPONENT_CONTEXTS))
                .SelectMany(d => d.GetContexts())
                .Select(contextName => contextName + COMPONENT_LOOKUP)
                .Select(lookupName => new CodeGenFile(
                    lookupName,
                    generateIndicesLookup(lookupName, emptyData),
                    generatorName
                )).ToArray();
        }

        CodeGenFile[] generateLookupsForContexts(CodeGeneratorData[] data) {
            var orderedData = data.OrderBy(d => d.GetShortTypeName()).ToArray();
            var lookupTagToComponentInfosMap = getLookupNameToDataMap(orderedData);
            var generatorName = GetType().FullName;
            return lookupTagToComponentInfosMap
                .Select(kv => new CodeGenFile(
                    kv.Key,
                    generateIndicesLookup(kv.Key, kv.Value.ToArray()),
                    generatorName
                )).ToArray();
        }

        static Dictionary<string, CodeGeneratorData[]> getLookupNameToDataMap(CodeGeneratorData[] data) {
            var lookupNameToData = data.Where(d => d.ShouldGenerateIndex())
                .Aggregate(new Dictionary<string, CodeGeneratorData[]>(), (map, d) => {
                    foreach(var contextName in d.GetContexts()) {
                        var lookupName = contextName + COMPONENT_LOOKUP;
                        if(!map.ContainsKey(lookupName)) {
                            map.Add(lookupName, new CodeGeneratorData[data.Length]);
                        }
                        var entries = map[lookupName];
                        for(int i = 0; i < entries.Length; i++) {
                            if(entries[i] == null) {
                                entries[i] = d;
                                break;
                            }
                        }
                    }

                    return map;
                });


            foreach(var key in lookupNameToData.Keys.ToArray()) {
                var dataList = lookupNameToData[key].ToList();
                while(dataList.Count != 0) {
                    var last = dataList.Count - 1;
                    if(dataList[last] == null) {
                        dataList.RemoveAt(last);
                    } else {
                        break;
                    }
                }
                lookupNameToData[key] = dataList.ToArray();
            }

            return lookupNameToData;
        }

        static string generateIndicesLookup(string lookupTag, CodeGeneratorData[] data) {
            return addClassHeader(lookupTag)
                    + addIndices(data)
                    + addComponentNames(data)
                    + addComponentTypes(data)
                    + addCloseClass();
        }

        static string addClassHeader(string lookupTag) {
            return string.Format("public static class {0} {{\n", lookupTag);
        }

        static string addIndices(CodeGeneratorData[] data) {
            const string fieldFormat = "    public const int {0} = {1};\n";
            const string totalFormat = "    public const int TotalComponents = {0};";
            var code = string.Empty;
            for(int i = 0; i < data.Length; i++) {
                var info = data[i];
                if(info != null) {
                    code += string.Format(fieldFormat, info.GetShortTypeName().RemoveComponentSuffix(), i);
                }
            }

            if(code.Length != 0) {
                code = "\n" + code;
            }

            var totalComponents = string.Format(totalFormat, data.Length);
            return code + "\n" + totalComponents;
        }

        static string addComponentNames(CodeGeneratorData[] data) {
            const string format = "        \"{1}\",\n";
            const string nullFormat = "        null,\n";
            var code = string.Empty;
            for(int i = 0; i < data.Length; i++) {
                var info = data[i];
                if(info != null) {
                    code += string.Format(format, i, info.GetShortTypeName().RemoveComponentSuffix());
                } else {
                    code += nullFormat;
                }
            }
            if(code.EndsWith(",\n", System.StringComparison.Ordinal)) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly string[] componentNames = {{
{0}    }};", code);
        }

        static string addComponentTypes(CodeGeneratorData[] data) {
            const string format = "        typeof({1}),\n";
            const string nullFormat = "        null,\n";
            var code = string.Empty;
            for(int i = 0; i < data.Length; i++) {
                var info = data[i];
                if(info != null) {
                    code += string.Format(format, i, info.GetFullTypeName());
                } else {
                    code += nullFormat;
                }
            }
            if(code.EndsWith(",\n", System.StringComparison.Ordinal)) {
                code = code.Remove(code.Length - 2) + "\n";
            }

            return string.Format(@"

    public static readonly System.Type[] componentTypes = {{
{0}    }};", code);
        }

        static string addCloseClass() {
            return "\n}\n";
        }
    }
}
