using System.IO;
using System.Linq;
using Jenny;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class CleanupSystemsGenerator : ICodeGenerator
    {
        public string Name => "Cleanup (Systems)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed class ${Context}CleanupSystems : Feature {

    public ${Context}CleanupSystems(Contexts contexts) {
${systemsList}
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<CleanupData>()
            .GroupBy(d => d.ComponentData.Context)
            .Select(group => Generate(group.Key, group.ToArray()))
            .ToArray();

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
