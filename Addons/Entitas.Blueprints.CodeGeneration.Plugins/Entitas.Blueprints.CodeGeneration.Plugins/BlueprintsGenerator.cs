using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.Blueprints.CodeGeneration.Plugins {

    public class BlueprintsGenerator : ICodeGenerator {

        public string name { get { return "Blueprint"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

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

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var blueprintNames = data
                .OfType<BlueprintData>()
                .Select(d => d.GetBlueprintName())
                .OrderBy(name => name)
                .ToArray();

            if (blueprintNames.Length == 0) {
                return new CodeGenFile[0];
            }

            var blueprints = CLASS_TEMPLATE.Replace("${blueprints}", generateBlueprintGetters(blueprintNames));
            return new[] {
                new CodeGenFile(
                    "GeneratedBlueprints.cs",
                    blueprints,
                    GetType().FullName)
            };
        }

        string generateBlueprintGetters(string[] blueprintNames) {
            return string.Join("\n", blueprintNames
                .Select(name => GETTER_TEMPLATE
                    .Replace("${ValidPropertyName}", validPropertyName(name))
                    .Replace("${Name}", name))
                .ToArray());
        }

        static string validPropertyName(string name) {
            return name
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);
        }
    }
}
