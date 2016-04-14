using System.Linq;
using Entitas.CodeGenerator;
using Entitas.Serialization.Blueprints;

namespace Entitas.CodeGenerator {
    
    public class BlueprintsGenerator : IBlueprintsCodeGenerator {

        const string FILE_NAME = "BlueprintsGeneratedExtension";

        const string CLASS_FORMAT = @"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {{
    public partial class Blueprints {{
{0}
    }}
}}
";
        const string GETTER_FORMAT = "        public Blueprint {0} {{ get {{ return GetBlueprint(\"{1}\"); }} }}";

        public CodeGenFile[] Generate(string[] blueprintNames) {
            if (blueprintNames.Length == 0) {
                return new CodeGenFile[0];
            }

            var generatorName = typeof(BlueprintsGenerator).FullName;
            var orderedBlueprintNames = blueprintNames.OrderBy(name => name).ToArray();
            var file = new CodeGenFile {
                fileName = FILE_NAME,
                fileContent = string
                    .Format(CLASS_FORMAT, generateBlueprintGetters(orderedBlueprintNames))
                    .ToUnixLineEndings(),
                generatorName = generatorName
            };

            return new [] { file };
        }

        string generateBlueprintGetters(string[] blueprintNames) {
            return string.Join("\n", blueprintNames
                .Select(name => string.Format(GETTER_FORMAT, validPropertyName(name), name)).ToArray());
        }

        string validPropertyName(string name) {
            return name
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);
        }
    }
}

