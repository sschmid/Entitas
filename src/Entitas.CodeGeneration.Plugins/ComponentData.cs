using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentData : CodeGeneratorData
    {
        public ComponentData() { }

        public ComponentData(CodeGeneratorData data) : base(data) { }
    }

    public static class ComponentDataExtension
    {
        public static string ToComponentName(this string fullTypeName, bool ignoreNamespaces) => ignoreNamespaces
            ? fullTypeName.TypeName().RemoveComponentSuffix()
            : fullTypeName.RemoveDots().RemoveComponentSuffix();
    }
}
