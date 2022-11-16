using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentEntityApiInterfaceGenerator : AbstractGenerator
    {
        public override string Name => "Component (Entity API Interface)";

        const string StandardTemplate =
            @"public partial interface I${ComponentName}Entity {

    ${ComponentType} ${validComponentName} { get; }
    bool has${ComponentName} { get; }

    void Add${ComponentName}(${newMethodParameters});
    void Replace${ComponentName}(${newMethodParameters});
    void Remove${ComponentName}();
}
";

        const string FlagTemplate =
            @"public partial interface I${ComponentName}Entity {
    bool ${prefixedComponentName} { get; set; }
}
";

        const string EntityInterfaceTemplate = "public partial class ${EntityType} : I${ComponentName}Entity { }\n";

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
                Path.Combine("Components", "Interfaces", $"I{data.Type.ToComponentName()}Entity.cs"),
                template.Replace(data, string.Empty),
                GetType().FullName
            );
        }

        CodeGenFile GenerateEntityInterface(string context, ComponentData data) => new CodeGenFile(
            Path.Combine(context, "Components", $"{data.ComponentNameWithContext(context).AddComponentSuffix()}.cs"),
            EntityInterfaceTemplate.Replace(data, context),
            GetType().FullName
        );
    }
}
