using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentEntityApiInterfaceGenerator : AbstractGenerator
    {
        public override string Name => "Component (Entity API Interface)";

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

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.Generates)
            .Where(d => d.Contexts.Length > 1)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => new[] {GenerateInterface(data)}
            .Concat(data.Contexts.Select(context => GenerateEntityInterface(context, data)));

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

        CodeGenFile GenerateEntityInterface(string context, ComponentData data) => new CodeGenFile(
            Path.Combine(context, "Components", $"{context + data.Name.AddComponentSuffix()}.cs"),
            data.ReplacePlaceholders(EntityInterfaceTemplate.Replace(data, context)),
            GetType().FullName
        );
    }
}
