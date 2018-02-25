using System.Collections.Generic;
using System.Linq;
using System.IO;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentLookupGenerator : AbstractGenerator {

        public override string name { get { return "Component (Lookup)"; } }

        const string TEMPLATE =
            @"public static class ${Lookup} {

${componentConstantsList}

${totalComponentsConstant}

    public static readonly string[] componentNames = {
${componentNamesList}
    };

    public static readonly System.Type[] componentTypes = {
${componentTypesList}
    };
}
";

        const string COMPONENT_CONSTANT_TEMPLATE = @"    public const int ${ComponentName} = ${Index};";
        const string TOTAL_COMPONENTS_CONSTANT_TEMPLATE = @"    public const int TotalComponents = ${totalComponents};";
        const string COMPONENT_NAME_TEMPLATE = @"        ""${ComponentName}""";
        const string COMPONENT_TYPE_TEMPLATE = @"        typeof(${ComponentType})";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var lookups = generateLookups(data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .ToArray());

            var existingFileNames = new HashSet<string>(lookups.Select(file => file.fileName));

            var emptyLookups = generateEmptyLookups(data
                    .OfType<ContextData>()
                    .ToArray())
                .Where(file => !existingFileNames.Contains(file.fileName))
                .ToArray();

            return lookups.Concat(emptyLookups).ToArray();
        }

        CodeGenFile[] generateEmptyLookups(ContextData[] data) {
            var emptyData = new ComponentData[0];
            return data
                .Select(d => generateComponentsLookupClass(d.GetContextName(), emptyData))
                .ToArray();
        }

        CodeGenFile[] generateLookups(ComponentData[] data) {
            var contextNameToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) => {
                    var contextNames = d.GetContextNames();
                    foreach (var contextName in contextNames) {
                        if (!dict.ContainsKey(contextName)) {
                            dict.Add(contextName, new List<ComponentData>());
                        }

                        dict[contextName].Add(d);
                    }

                    return dict;
                });

            foreach (var key in contextNameToComponentData.Keys.ToArray()) {
                contextNameToComponentData[key] = contextNameToComponentData[key]
                    .OrderBy(d => d.GetTypeName())
                    .ToList();
            }

            return contextNameToComponentData
                .Select(kv => generateComponentsLookupClass(kv.Key, kv.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile generateComponentsLookupClass(string contextName, ComponentData[] data) {
            var componentConstantsList = string.Join("\n", data
                .Select((d, index) => COMPONENT_CONSTANT_TEMPLATE
                    .Replace("${ComponentName}", d.ComponentName())
                    .Replace("${Index}", index.ToString())).ToArray());

            var totalComponentsConstant = TOTAL_COMPONENTS_CONSTANT_TEMPLATE
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNamesList = string.Join(",\n", data
                .Select(d => COMPONENT_NAME_TEMPLATE
                    .Replace("${ComponentName}", d.ComponentName())
                ).ToArray());

            var componentTypesList = string.Join(",\n", data
                .Select(d => COMPONENT_TYPE_TEMPLATE
                    .Replace("${ComponentType}", d.GetTypeName())
                ).ToArray());

            var fileContent = TEMPLATE
                .Replace("${Lookup}", contextName + CodeGeneratorExtentions.LOOKUP)
                .Replace("${componentConstantsList}", componentConstantsList)
                .Replace("${totalComponentsConstant}", totalComponentsConstant)
                .Replace("${componentNamesList}", componentNamesList)
                .Replace("${componentTypesList}", componentTypesList);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                contextName + "ComponentsLookup.cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
