using System.Linq;

namespace Entitas.CodeGenerator {

    public class BlueprintsGenerator : ICodeGenerator {

        const string CLASS_FORMAT = @"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {{

    public partial class Blueprints {{

{0}
    }}
}}
";
        const string GETTER_FORMAT = "        public Blueprint {0} {{ get {{ return GetBlueprint(\"{1}\"); }} }}";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            const string blueprintName = BlueprintDataProvider.BLUEPRINT_NAME;
            var blueprintNames = data
                .Where(d => d.ContainsKey(blueprintName))
                .Select(d => d.GetBlueprintName())
                .OrderBy(name => name)
                .ToArray();

            if(blueprintNames.Length == 0) {
                return new CodeGenFile[0];
            }

            var blueprints = string.Format(CLASS_FORMAT, generateBlueprintGetters(blueprintNames));
            return new [] { new CodeGenFile(
                "BlueprintsGeneratedExtension",
                blueprints,
                GetType().FullName) };
        }

        string generateBlueprintGetters(string[] blueprintNames) {
            return string.Join("\n", blueprintNames
                .Select(name => string.Format(GETTER_FORMAT, validPropertyName(name), name)).ToArray());
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
