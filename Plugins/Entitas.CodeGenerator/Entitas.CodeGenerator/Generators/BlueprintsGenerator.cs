using System.Linq;

namespace Entitas.CodeGenerator {

    public class BlueprintsGenerator : ICodeGenerator {

        const string CLASS_TEMPLATE =
@"using Entitas.Blueprints;

namespace Entitas.Unity.Blueprints {{

    public partial class Blueprints {{

{0}
    }}
}}
";
        const string GETTER_TEMPLATE = "        public Blueprint {0} {{ get {{ return GetBlueprint(\"{1}\"); }} }}";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var blueprintNames = data
                .OfType<BlueprintData>()
                .Select(d => d.GetBlueprintName())
                .OrderBy(name => name)
                .ToArray();

            if(blueprintNames.Length == 0) {
                return new CodeGenFile[0];
            }

            var blueprints = string.Format(CLASS_TEMPLATE, generateBlueprintGetters(blueprintNames));
            return new[] { new CodeGenFile(
                "BlueprintsGeneratedExtension",
                blueprints,
                GetType().FullName)
            };
        }

        string generateBlueprintGetters(string[] blueprintNames) {
            return string.Join("\n", blueprintNames
                .Select(name => string.Format(GETTER_TEMPLATE, validPropertyName(name), name)).ToArray());
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
