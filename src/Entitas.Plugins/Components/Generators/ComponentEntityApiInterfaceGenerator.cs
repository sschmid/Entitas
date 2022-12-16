using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentEntityApiInterfaceGenerator : ICodeGenerator
    {
        public string Name => "Component (Entity API Interface)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string StandardTemplate =
            @"public partial interface I${Component.Name}Entity
{
    ${Component.Type} ${Component.Name} { get; }
    bool Has${Component.Name} { get; }

    I${Component.Name}Entity Add${Component.Name}(${newMethodParameters});
    I${Component.Name}Entity Replace${Component.Name}(${newMethodParameters});
    I${Component.Name}Entity Remove${Component.Name}();
}
";

        const string FlagTemplate =
            @"public partial interface I${Component.Name}Entity
{
    bool ${PrefixedComponentName} { get; set; }
}
";

        const string EntityInterfaceTemplate = "public partial class ${Context.Entity.Type} : I${Component.Name}Entity { }\n";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.Generates)
            .GroupBy(d => d.Type)
            .Select(group => group.ToArray())
            .Where(datas => datas.Length > 1)
            .SelectMany(datas => datas.SelectMany(d => Generate(d)))
            .ToArray();

        CodeGenFile[] Generate(ComponentData data) => new[]
        {
            GenerateInterface(data),
            GenerateEntityInterface(data),
        };

        CodeGenFile GenerateInterface(ComponentData data)
        {
            var template = data.MemberData.Length == 0
                ? FlagTemplate
                : StandardTemplate;

            return new CodeGenFile(
                Path.Combine("Components", "Interfaces", $"I{data.Name}Entity.cs"),
                data.ReplacePlaceholders(template.Replace(data, string.Empty)),
                GetType().FullName
            );
        }

        CodeGenFile GenerateEntityInterface(ComponentData data) => new CodeGenFile(
            Path.Combine(data.Context, "Components", $"{data.Context + data.Name.AddComponentSuffix()}.cs"),
            data.ReplacePlaceholders(EntityInterfaceTemplate.Replace(data, data.Context)),
            GetType().FullName
        );
    }
}
