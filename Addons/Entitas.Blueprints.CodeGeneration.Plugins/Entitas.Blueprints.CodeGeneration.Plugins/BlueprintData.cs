using DesperateDevs.CodeGeneration;

namespace Entitas.Blueprints.CodeGeneration.Plugins {

    public class BlueprintData : CodeGeneratorData {
    }

    public static class BlueprintDataExtension {

        public const string BLUEPRINT_NAME = "blueprint_name";

        public static string GetBlueprintName(this BlueprintData data) {
            return (string)data[BLUEPRINT_NAME];
        }

        public static void SetBlueprintName(this BlueprintData data, string blueprintName) {
            data[BLUEPRINT_NAME] = blueprintName;
        }
    }
}
