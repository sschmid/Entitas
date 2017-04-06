using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentData : CodeGeneratorData {
    }

    public static class ComponentDataExtension {

        public static string ToComponentName(this string fullTypeName) {
            return fullTypeName.RemoveDots().RemoveComponentSuffix();
        }
    }
}
