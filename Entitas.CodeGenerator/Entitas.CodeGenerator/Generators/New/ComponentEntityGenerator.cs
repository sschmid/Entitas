using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentEntityGenerator : ICodeGenerator {

        const string normalComponentTemplate =
@"using Entitas;

public sealed partial class ${Context}Entity : Entity {

    public ${Name}Component ${name} { get { return (${Type})GetComponent(${Index}); } }
    public bool has${Name} { get { return HasComponent(${Index}); } }

    public void Add${Name}(${memberArgs}) {
        var component = CreateComponent<${Type}>(${Index});
${memberAssignment}
        AddComponent(${Index}, component);
    }

    public void Replace${Name}(${memberArgs}) {
        var component = CreateComponent<${Type}>(${Index});
${memberAssignment}
        ReplaceComponent(${Index}, component);
    }

    public void Remove${Name}() {
        RemoveComponent(${Index});
    }
}
";

        const string memberArgsTemplate =
@"${MemberType} new${MemberName}";

        const string memberAssignmentTemplate =
@"        component.${memberName} = new${MemberName};";

        const string flagComponentTemplate =
@"using Entitas;

public sealed partial class ${Context}Entity : Entity {

    static readonly ${Name}Component ${name}Component = new ${Name}Component();

    public bool is${Name} {
        get { return HasComponent(${Index}); }
        set {
            if(value != is${Name}) {
                if(value) {
                    AddComponent(${Index}, ${name}Component);
                } else {
                    RemoveComponent(${Index});
                }
            }
        }
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .Select(d => generateExtensions(d))
                .SelectMany(files => files)
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                       .Select(contextName => generateExtension(contextName, data))
                       .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var fullComponentName = data.GetShortTypeName();
            var shortComponentName = fullComponentName.RemoveComponentSuffix();
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + shortComponentName.ToUpper();
            var memberInfos = data.GetMemberInfos();

            var template = memberInfos.Count == 0
                                      ? flagComponentTemplate
                                      : normalComponentTemplate;

            var fileContent = template
                .Replace("${Context}", contextName)
                .Replace("${Name}", shortComponentName)
                .Replace("${name}", shortComponentName.LowercaseFirst())
                .Replace("${Type}", data.GetFullTypeName())
                .Replace("${Index}", index)
                .Replace("${memberArgs}", getMemberArgs(memberInfos))
                .Replace("${memberAssignment}", getMemberAssignment(memberInfos));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + "Components" +
                Path.DirectorySeparatorChar + contextName + fullComponentName + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(List<PublicMemberInfo> memberInfos) {
            var args = memberInfos
                .Select(info => memberArgsTemplate
                        .Replace("${MemberType}", info.type.FullName)
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join(", ", args);
        }

        string getMemberAssignment(List<PublicMemberInfo> memberInfos) {
            var assignments = memberInfos
                .Select(info => memberAssignmentTemplate
                        .Replace("${MemberType}", info.type.FullName)
                        .Replace("${memberName}", info.name)
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join("\n", assignments);
        }
    }
}
