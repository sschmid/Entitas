using System.IO;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.Plugins
{
    public class ComponentGenerator : ICodeGenerator
    {
        public string Name => "Component";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string ComponentTemplate =
            @"[Entitas.Plugins.Attributes.DontGenerate(false)]
public sealed class ${FullComponentName} : Entitas.IComponent {
    public ${Type} Value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.GeneratesObject)
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ComponentData data)
        {
            var fullComponentName = data.Type.RemoveDots();
            return new CodeGenFile(
                Path.Combine("Components", $"{fullComponentName}.cs"),
                ComponentTemplate
                    .Replace("${FullComponentName}", fullComponentName)
                    .Replace("${Type}", data.ObjectType),
                GetType().FullName
            );
        }
    }
}
