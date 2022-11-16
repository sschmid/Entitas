using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentLookupGenerator : AbstractGenerator
    {
        public override string Name => "Component (Lookup)";

        const string Template =
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

        const string ComponentConstantTemplate = @"    public const int ${ComponentName} = ${Index};";
        const string TotalComponentsConstantTemplate = @"    public const int TotalComponents = ${totalComponents};";
        const string ComponentNameTemplate = @"        ""${ComponentName}""";
        const string ComponentTypeTemplate = @"        typeof(${ComponentType})";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var lookups = GenerateLookups(data
                .OfType<ComponentData>()
                .Where(d => d.GeneratesIndex)
                .ToArray());

            var existingFileNames = new HashSet<string>(lookups.Select(file => file.FileName));

            var emptyLookups = GenerateEmptyLookups(data
                    .OfType<ContextData>()
                    .ToArray())
                .Where(file => !existingFileNames.Contains(file.FileName))
                .ToArray();

            return lookups.Concat(emptyLookups).ToArray();
        }

        CodeGenFile[] GenerateEmptyLookups(ContextData[] data)
        {
            var emptyData = Array.Empty<ComponentData>();
            return data
                .Select(d => GenerateComponentsLookupClass(d.Name, emptyData))
                .ToArray();
        }

        CodeGenFile[] GenerateLookups(ComponentData[] data)
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
                .Select(kvp => GenerateComponentsLookupClass(kvp.Key, kvp.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile GenerateComponentsLookupClass(string context, ComponentData[] data)
        {
            var componentConstantsList = string.Join("\n", data
                .Select((d, index) => ComponentConstantTemplate
                    .Replace("${ComponentName}", d.Type.ToComponentName())
                    .Replace("${Index}", index.ToString())).ToArray());

            var totalComponentsConstant = TotalComponentsConstantTemplate
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNamesList = string.Join(",\n", data
                .Select(d => ComponentNameTemplate.Replace("${ComponentName}", d.Type.ToComponentName())));

            var componentTypesList = string.Join(",\n", data
                .Select(d => ComponentTypeTemplate.Replace("${ComponentType}", d.Type)));

            var fileContent = Template
                .Replace("${Lookup}", context + CodeGeneratorExtensions.ComponentLookup)
                .Replace("${componentConstantsList}", componentConstantsList)
                .Replace("${totalComponentsConstant}", totalComponentsConstant)
                .Replace("${componentNamesList}", componentNamesList)
                .Replace("${componentTypesList}", componentTypesList);

            return new CodeGenFile(
                Path.Combine(context, $"{context}ComponentsLookup.cs"),
                fileContent,
                GetType().FullName
            );
        }
    }
}
