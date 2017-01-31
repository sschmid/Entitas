using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentEntityGenerator : ICodeGenerator {

        const string STANDARD_COMPONENT_TEMPLATE =
@"public partial class ${Context}Entity {

    public ${Type} ${name} { get { return (${Type})GetComponent(${Index}); } }
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

        const string MEMBER_ARGS_TEMPLATE =
@"${MemberType} new${MemberName}";

        const string MEMBER_ASSIGNMENT_TEMPLATE =
@"        component.${memberName} = new${MemberName};";

        const string FLAG_COMPONENT_TEMPLATE =
@"public partial class ${Context}Entity {

    static readonly ${Type} ${name}Component = new ${Type}();

    public bool ${prefixedName} {
        get { return HasComponent(${Index}); }
        set {
            if(value != ${prefixedName}) {
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
                .SelectMany(d => generateExtensions(d))
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                       .Select(contextName => generateExtension(contextName, data))
                       .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + data.GetComponentName();
            var memberInfos = data.GetMemberInfos();
            var template = memberInfos.Count == 0
                                      ? FLAG_COMPONENT_TEMPLATE
                                      : STANDARD_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetComponentName())
                .Replace("${name}", data.GetComponentName().LowercaseFirst())
                .Replace("${prefixedName}", data.GetUniqueComponentPrefix().LowercaseFirst() + data.GetComponentName())
                .Replace("${Type}", data.GetFullTypeName())
                .Replace("${Index}", index)
                .Replace("${memberArgs}", getMemberArgs(memberInfos))
                .Replace("${memberAssignment}", getMemberAssignment(memberInfos));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + "Components" +
                Path.DirectorySeparatorChar + contextName + data.GetFullComponentName() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(List<PublicMemberInfo> memberInfos) {
            var args = memberInfos
                .Select(info => MEMBER_ARGS_TEMPLATE
                        .Replace("${MemberType}", info.type.ToCompilableString())
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join(", ", args);
        }

        string getMemberAssignment(List<PublicMemberInfo> memberInfos) {
            var assignments = memberInfos
                .Select(info => MEMBER_ASSIGNMENT_TEMPLATE
                        .Replace("${MemberType}", info.type.ToCompilableString())
                        .Replace("${memberName}", info.name)
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join("\n", assignments);
        }
    }
}
