using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentGenerator : ICodeGenerator {

        const string COMPONENT_TEMPLATE =
@"using Entitas;

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
            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar + data.GetFullComponentName() + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${Name}", data.GetFullComponentName())
                    .Replace("${Type}", data.GetObjectType()),
                GetType().FullName
            );
        }
    }
}
