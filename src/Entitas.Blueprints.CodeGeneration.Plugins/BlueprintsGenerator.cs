using System;
using System.Linq;
using Jenny;

namespace Entitas.Blueprints.CodeGeneration.Plugins
{
    public class BlueprintsGenerator : ICodeGenerator
    {
        public string Name => "Blueprint";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string CLASS_TEMPLATE =
            @"using Entitas.Blueprints;
using Entitas.Blueprints.Unity;

public static class BlueprintsExtension {

${blueprints}
}
";

        const string GETTER_TEMPLATE =
            @"    public static Blueprint ${ValidPropertyName}(this Blueprints blueprints) {
        return blueprints.GetBlueprint(""${Name}"");
    }";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var blueprintNames = data
                .OfType<BlueprintData>()
                .Select(d => d.GetBlueprintName())
                .OrderBy(name => name)
                .ToArray();

            if (blueprintNames.Length == 0)
                return Array.Empty<CodeGenFile>();

            var blueprints = CLASS_TEMPLATE.Replace("${blueprints}", generateBlueprintGetters(blueprintNames));
            return new[]
            {
                new CodeGenFile(
                    "GeneratedBlueprints.cs",
                    blueprints,
                    GetType().FullName)
            };
        }

        string generateBlueprintGetters(string[] blueprintNames) => string.Join("\n", blueprintNames
            .Select(name => GETTER_TEMPLATE
                .Replace("${ValidPropertyName}", validPropertyName(name))
                .Replace("${Name}", name)));

        static string validPropertyName(string name) => name
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("(", string.Empty)
            .Replace(")", string.Empty);
    }
}
