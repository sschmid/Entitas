using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.Unity.Blueprints {

    public class BlueprintDataProvider : ICodeGeneratorDataProvider {

        public string name { get { return "Blueprint"; } }
        public bool isEnabledByDefault { get { return true; } }

        readonly string[] _blueprintNames;

        public BlueprintDataProvider() {
            _blueprintNames = BinaryBlueprintInspector
                .FindAllBlueprints()
                .Select(b => b.Deserialize().name)
                .ToArray();
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
