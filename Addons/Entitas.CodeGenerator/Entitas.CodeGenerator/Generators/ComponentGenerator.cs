using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentGenerator : ICodeGenerator {

        public string name { get { return "Component"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string COMPONENT_TEMPLATE =
@"${Contexts}${Unique}
public sealed partial class ${Name} : Entitas.IComponent {
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

            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar + data.GetFullComponentName() + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${Name}", data.GetFullComponentName())
                    .Replace("${Type}", data.GetObjectType())
                    .Replace("${Contexts}", contexts)
                    .Replace("${Unique}", unique),
                GetType().FullName
            );
        }
    }
}
