using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentGenerator : ICodeGenerator {

        public bool IsEnabledByDefault { get { return true; } }

        const string COMPONENT_TEMPLATE =
@"using Entitas;

${Contexts}${Unique}${HideInBlueprintsInspector}
public sealed partial class ${Name} : IComponent {
    public ${Type} value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateComponent())
                .Select(d => generateComponentClass(d))
                .ToArray();
        }

        CodeGenFile generateComponentClass(ComponentData data) {
            var contexts = string.Join(", ", data.GetContextNames());
            var unique = data.IsUnique() ? "[Entitas.CodeGenerator.Api.UniqueAttribute]" : string.Empty;
            if(!string.IsNullOrEmpty(contexts)) {
                contexts = "[" + contexts + "]";
            }

            var hide = data.ShouldHideInBlueprintInspector()
                           ? "[Entitas.CodeGenerator.Api.HideInBlueprintInspectorAttribute]"
                           : string.Empty;

            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar + data.GetFullComponentName() + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${Name}", data.GetFullComponentName())
                    .Replace("${Type}", data.GetObjectType())
                    .Replace("${Contexts}", contexts)
                    .Replace("${Unique}", unique)
                    .Replace("${HideInBlueprintsInspector}", hide),
                GetType().FullName
            );
        }
    }
}
