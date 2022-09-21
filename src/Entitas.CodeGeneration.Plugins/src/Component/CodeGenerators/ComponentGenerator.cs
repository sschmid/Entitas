using System.IO;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentGenerator : ICodeGenerator
    {
        public string Name => "Component";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string COMPONENT_TEMPLATE =
            @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${FullComponentName} : Entitas.IComponent {
    public ${Type} value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.ShouldGenerateComponent())
            .Select(generate)
            .ToArray();

        CodeGenFile generate(ComponentData data)
        {
            var fullComponentName = data.GetTypeName().RemoveDots();
            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar +
                fullComponentName + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${FullComponentName}", fullComponentName)
                    .Replace("${Type}", data.GetObjectTypeName()),
                GetType().FullName
            );
        }
    }
}
