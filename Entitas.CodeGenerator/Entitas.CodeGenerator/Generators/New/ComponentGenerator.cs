using System.IO;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentGenerator : ICodeGenerator {

        const string componentTemplate =
@"using Entitas.Api;

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
            var name = data.GetShortTypeName().AddComponentSuffix();
            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar + name + ".cs",
                componentTemplate
                    .Replace("${Name}", name)
                    .Replace("${Type}", data.GetFullTypeName()),
                GetType().FullName
            );
        }
    }
}
