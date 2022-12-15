using System.Collections.Generic;
using System.Linq;
using System.IO;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentLookupGenerator : AbstractGenerator
    {
        public override string Name => "Component (Lookup)";

        const string Template =
            @"public static class ${Lookup}
{
${ComponentConstantsList}

${TotalComponentsConstant}

    public static readonly string[] ComponentNames =
    {
${ComponentNamesList}
    };

    public static readonly System.Type[] ComponentTypes =
    {
${ComponentTypesList}
    };
}
";

        const string ComponentConstantTemplate = @"    public const int ${Component.Name} = ${Index};";
        const string TotalComponentsConstantTemplate = @"    public const int TotalComponents = ${TotalComponents};";
        const string ComponentNameTemplate = @"        ""${Component.Name}""";
        const string ComponentTypeTemplate = @"        typeof(${Component.Type})";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var lookups = GenerateLookups(data
                .OfType<ComponentData>()
                .Where(d => d.GeneratesIndex));

            var existingFileNames = new HashSet<string>(lookups.Select(file => file.FileName));
            var emptyLookups = GenerateEmptyLookups(data.OfType<ContextData>())
                .Where(file => !existingFileNames.Contains(file.FileName));

            return lookups.Concat(emptyLookups).ToArray();
        }

        IEnumerable<CodeGenFile> GenerateEmptyLookups(IEnumerable<ContextData> data)
        {
            var emptyData = new List<ComponentData>(0);
            return data.Select(d => GenerateComponentsLookupClass(d.Name, emptyData));
        }

        CodeGenFile[] GenerateLookups(IEnumerable<ComponentData> data)
        {
            var contextToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) =>
                {
                    var contexts = d.Contexts;
                    foreach (var context in contexts)
                    {
                        if (!dict.ContainsKey(context))
                            dict.Add(context, new List<ComponentData>());

                        dict[context].Add(d);
                    }

                    return dict;
                });

            foreach (var key in contextToComponentData.Keys.ToArray())
            {
                contextToComponentData[key] = contextToComponentData[key]
                    .OrderBy(d => d.Type)
                    .ToList();
            }

            return contextToComponentData
                .Select(kvp => GenerateComponentsLookupClass(kvp.Key, kvp.Value))
                .ToArray();
        }

        CodeGenFile GenerateComponentsLookupClass(string context, List<ComponentData> data)
        {
            var componentConstantsList = string.Join("\n", data.Select((d, index) => d
                .ReplacePlaceholders(ComponentConstantTemplate)
                .Replace("${Index}", index.ToString())));

            var totalComponentsConstant = TotalComponentsConstantTemplate
                .Replace("${TotalComponents}", data.Count.ToString());

            var componentNamesList = string.Join(",\n", data.Select(d =>
                d.ReplacePlaceholders(ComponentNameTemplate)));

            var componentTypesList = string.Join(",\n", data.Select(d =>
                d.ReplacePlaceholders(ComponentTypeTemplate)));

            var fileContent = Template
                .Replace("${Lookup}", context + CodeGeneratorExtensions.ComponentLookup)
                .Replace("${ComponentConstantsList}", componentConstantsList)
                .Replace("${TotalComponentsConstant}", totalComponentsConstant)
                .Replace("${ComponentNamesList}", componentNamesList)
                .Replace("${ComponentTypesList}", componentTypesList);

            return new CodeGenFile(
                Path.Combine(context, $"{context}ComponentsLookup.cs"),
                fileContent,
                GetType().FullName
            );
        }
    }
}
