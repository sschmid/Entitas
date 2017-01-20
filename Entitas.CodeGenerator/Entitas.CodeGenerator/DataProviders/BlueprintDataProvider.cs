using System.Linq;

namespace Entitas.CodeGenerator {

    public static class BlueprintCodeGeneratorDataExtension {

        public static string GetBlueprintName(this CodeGeneratorData data) {
            return (string)data[BlueprintDataProvider.BLUEPRINT_NAME];
        }
    }

    public class BlueprintDataProvider : ICodeGeneratorDataProvider {

        public const string BLUEPRINT_NAME = "blueprint_name";

        readonly string[] _blueprintNames;

        public BlueprintDataProvider(string[] blueprintNames) {
            _blueprintNames = blueprintNames;
        }

        public CodeGeneratorData[] GetData() {
            return _blueprintNames
                .Select(blueprintName => {
                    var data = new CodeGeneratorData(GetType().FullName);
                    data[BLUEPRINT_NAME] = blueprintName;
                    return data;
                }).ToArray();
        }
    }
}
