using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Roslyn.CodeGeneration.Plugins {

    public class CleanupSystemsGenerator : AbstractGenerator {

        public override string Name => "Cleanup (Systems)";

        const string TEMPLATE =
            @"public sealed class ${ContextName}CleanupSystems : Feature {

    public ${ContextName}CleanupSystems(Contexts contexts) {
${systemsList}
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var cleanupData = data
                .OfType<CleanupData>()
                .ToArray();

            var contextNameToCleanupData = cleanupData
                .Aggregate(new Dictionary<string, List<CleanupData>>(), (dict, d) => {
                    var contextNames = d.componentData.GetContextNames();
                    foreach (var contextName in contextNames) {
                        if (!dict.ContainsKey(contextName)) {
                            dict.Add(contextName, new List<CleanupData>());
                        }

                        dict[contextName].Add(d);
                    }

                    return dict;
                });

            return generate(contextNameToCleanupData);
        }

        CodeGenFile[] generate(Dictionary<string, List<CleanupData>> contextNameToCleanupData) {
            return contextNameToCleanupData
                .Select(kv => generate(kv.Key, kv.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile generate(string contextName, CleanupData[] data) {
            var systemsList = string.Join("\n", data
                .Select(d => "        Add(new " +
                             (d.cleanupMode == CleanupMode.DestroyEntity ? "Destroy" : "Remove") +
                             d.componentData.ComponentName() + contextName.AddSystemSuffix() + "(contexts));")
                .ToArray());

            var fileContent = TEMPLATE
                .Replace("${systemsList}", systemsList)
                .Replace(contextName);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                contextName + "CleanupSystems.cs",
                fileContent,
                GetType().FullName);
        }
    }
}
