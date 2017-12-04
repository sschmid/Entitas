using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentData : CodeGeneratorData {
    }

    public static class ComponentDataExtension {

        public static string ToComponentName(this string fullTypeName, bool ignoreNamespaces) {
            return ignoreNamespaces
                ? fullTypeName.ShortTypeName().RemoveComponentSuffix()
                : fullTypeName.RemoveDots().RemoveComponentSuffix();
        }
    }
}
