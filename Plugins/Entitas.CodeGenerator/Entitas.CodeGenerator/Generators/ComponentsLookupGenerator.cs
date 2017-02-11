using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Entitas.CodeGenerator {

    public class ComponentsLookupGenerator : ICodeGenerator {

        public string name { get { return "Components Lookup"; } }
        public bool isEnabledByDefault { get { return true; } }

        public const string COMPONENTS_LOOKUP = "ComponentsLookup";

        const string COMPONENTS_LOOKUP_TEMPLATE =
@"public static class ${Lookup} {

${componentConstants}

${totalComponentsConstant}

    public static readonly string[] componentNames = {
${componentNames}
    };

    public static readonly System.Type[] componentTypes = {
${componentTypes}
    };
}
";

        const string COMPONENT_CONSTANTS_TEMPLATE =
@"    public const int ${Name} = ${Index};";

        const string TOTAL_COMPONENTS_CONSTANT_TEMPLATE =
@"    public const int TotalComponents = ${totalComponents};";

        const string COMPONENT_NAMES_TEMPLATE =
@"        ""${Name}""";

        const string COMPONENT_TYPES_TEMPLATE =
@"        typeof(${Type})";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var files = new List<CodeGenFile>();

            var contextData = data
                .OfType<ContextData>()
                .ToArray();
            files.AddRange(generateEmptyLookup(contextData));

            var componentData = data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .ToArray();
            files.AddRange(generateLookup(componentData));

            return files.ToArray();
        }

        CodeGenFile[] generateEmptyLookup(ContextData[] data) {
            var emptyData = new ComponentData[0];
            return data
                .Select(d => generateComponentsLookupClass(d.GetContextName(), emptyData))
                .ToArray();
        }

        CodeGenFile[] generateLookup(ComponentData[] data) {
            var contextNameToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) => {
                    var contextNames = d.GetContextNames();
                    foreach(var contextName in contextNames) {
                        if(!dict.ContainsKey(contextName)) {
                            dict.Add(contextName, new List<ComponentData>());
                        }

                        dict[contextName].Add(d);
                    }

                    return dict;
                });

            foreach(var key in contextNameToComponentData.Keys.ToArray()) {
                contextNameToComponentData[key] = contextNameToComponentData[key]
                    .OrderBy(d => d.GetComponentName())
                    .ToList();
            }

            return contextNameToComponentData
                .Select(kv => generateComponentsLookupClass(kv.Key, kv.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile generateComponentsLookupClass(string contextName, ComponentData[] data) {
            var componentConstants = string.Join("\n", data
                .Select((d, index) => {
                    return COMPONENT_CONSTANTS_TEMPLATE
                    .Replace("${Name}", d.GetComponentName())
                    .Replace("${Index}", index.ToString());
                }).ToArray());

            var totalComponentsConstant = TOTAL_COMPONENTS_CONSTANT_TEMPLATE
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNames = string.Join(",\n", data
                .Select(d => COMPONENT_NAMES_TEMPLATE
                        .Replace("${Name}", d.GetComponentName())
                ).ToArray());

            var componentTypes = string.Join(",\n", data
                .Select(d => COMPONENT_TYPES_TEMPLATE
                    .Replace("${Type}", d.GetFullTypeName())
                ).ToArray());

            var fileContent = COMPONENTS_LOOKUP_TEMPLATE
                .Replace("${Lookup}", contextName + COMPONENTS_LOOKUP)
                .Replace("${componentConstants}", componentConstants)
                .Replace("${totalComponentsConstant}", totalComponentsConstant)
                .Replace("${componentNames}", componentNames)
                .Replace("${componentTypes}", componentTypes);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + COMPONENTS_LOOKUP + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
