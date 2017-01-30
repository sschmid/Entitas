using System.Linq;

namespace Entitas.CodeGenerator {

    public class BlueprintDataProvider : ICodeGeneratorDataProvider {

        readonly string[] _blueprintNames;

        public BlueprintDataProvider(string[] blueprintNames) {
            _blueprintNames = blueprintNames;
        }

        public CodeGeneratorData[] GetData() {
            return _blueprintNames
                .Select(blueprintName => {
                    var data = new BlueprintData();
                    data.SetBlueprintName(blueprintName);
                    return data;
                }).ToArray();
        }
    }

    public static class BlueprintDataProviderExtension {

        public const string BLUEPRINT_NAME = "blueprint_name";

        public static string GetBlueprintName(this BlueprintData data) {
            return (string)data[BLUEPRINT_NAME];
        }

        public static void SetBlueprintName(this BlueprintData data, string blueprintName) {
            data[BLUEPRINT_NAME] = blueprintName;
        }
    }
}
