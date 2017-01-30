using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Entitas.CodeGenerator {

    public class ComponentsLookupGenerator : ICodeGenerator {

        public const string COMPONENTS_LOOKUP = "ComponentsLookup";

        const string componentsLookupTemplate =
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

        const string componentConstantsTemplate =
@"    public const int ${Name} = ${Index};";

        const string totalComponentsConstantTemplate =
@"    public const int TotalComponents = ${totalComponents};";

        const string componentNamesTemplate =
@"        ""${Name}""";

        const string componentTypesTemplate =
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
                    return componentConstantsTemplate
                    .Replace("${Name}", d.GetComponentName())
                    .Replace("${Index}", index.ToString());
                }).ToArray());

            var totalComponentsConstant = totalComponentsConstantTemplate
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNames = string.Join(",\n", data
                .Select(d => componentNamesTemplate
                        .Replace("${Name}", d.GetComponentName())
                ).ToArray());

            var componentTypes = string.Join(",\n", data
                .Select(d => componentTypesTemplate
                    .Replace("${Type}", d.GetFullTypeName())
                ).ToArray());

            var fileContent = componentsLookupTemplate
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
