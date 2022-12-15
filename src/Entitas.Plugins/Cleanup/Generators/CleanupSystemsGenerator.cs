using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class CleanupSystemsGenerator : AbstractGenerator
    {
        public override string Name => "Cleanup (Systems)";

        const string Template =
            @"public sealed class ${Context}CleanupSystems : Feature {

    public ${Context}CleanupSystems(Contexts contexts) {
${systemsList}
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contextToCleanupData = data.OfType<CleanupData>()
                .Aggregate(new Dictionary<string, List<CleanupData>>(), (dict, d) =>
                {
                    var contexts = d.ComponentData.Contexts;
                    foreach (var context in contexts)
                    {
                        if (!dict.ContainsKey(context))
                            dict.Add(context, new List<CleanupData>());

                        dict[context].Add(d);
                    }

                    return dict;
                });

            return Generate(contextToCleanupData);
        }

        CodeGenFile[] Generate(Dictionary<string, List<CleanupData>> contextToCleanupData) => contextToCleanupData
            .Select(kvp => Generate(kvp.Key, kvp.Value.ToArray())).ToArray();

        CodeGenFile Generate(string context, CleanupData[] data)
        {
            var systemsList = string.Join("\n", data.Select(d =>
                "        Add(new " +
                (d.CleanupMode == CleanupMode.DestroyEntity ? "Destroy" : "Remove") +
                d.ComponentData.Name + context.AddSystemSuffix() + "(contexts));"));

            return new CodeGenFile(
                Path.Combine(context, $"{context}CleanupSystems.cs"),
                Template
                    .Replace("${systemsList}", systemsList)
                    .Replace(context),
                GetType().FullName);
        }
    }
}
