using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentEntityGenerator : ICodeGenerator {

        public string name { get { return "Component (Entity API)"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string STANDARD_COMPONENT_TEMPLATE =
@"public partial class ${ContextName}Entity {

    public ${ComponentType} ${componentName} { get { return (${ComponentType})GetComponent(${Index}); } }
    public bool has${ComponentName} { get { return HasComponent(${Index}); } }

    public void Add${ComponentName}(${memberArgs}) {
        var index = ${Index};
        var component = CreateComponent<${ComponentType}>(index);
${memberAssignment}
        AddComponent(index, component);
    }

    public void Replace${ComponentName}(${memberArgs}) {
        var index = ${Index};
        var component = CreateComponent<${ComponentType}>(index);
${memberAssignment}
        ReplaceComponent(index, component);
    }

    public void Remove${ComponentName}() {
        RemoveComponent(${Index});
    }
}
";

        const string MEMBER_ARGS_TEMPLATE =
@"${MemberType} new${MemberName}";

        const string MEMBER_ASSIGNMENT_TEMPLATE =
@"        component.${memberName} = new${MemberName};";

        const string FLAG_COMPONENT_TEMPLATE =
@"public partial class ${ContextName}Entity {

    static readonly ${ComponentType} ${componentName}Component = new ${ComponentType}();

    public bool ${prefixedName} {
        get { return HasComponent(${Index}); }
        set {
            if(value != ${prefixedName}) {
                if(value) {
                    AddComponent(${Index}, ${componentName}Component);
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
            var componentName = data.GetFullTypeName().ToComponentName();
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + componentName;
            var memberData = data.GetMemberData();
            var template = memberData.Length == 0
                                      ? FLAG_COMPONENT_TEMPLATE
                                      : STANDARD_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentType}", data.GetFullTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedName}", data.GetUniqueComponentPrefix().LowercaseFirst() + componentName)
                .Replace("${Index}", index)
                .Replace("${memberArgs}", getMemberArgs(memberData))
                .Replace("${memberAssignment}", getMemberAssignment(memberData));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + componentName.AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => MEMBER_ARGS_TEMPLATE
                        .Replace("${MemberType}", info.type)
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join(", ", args);
        }

        string getMemberAssignment(MemberData[] memberData) {
            var assignments = memberData
                .Select(info => MEMBER_ASSIGNMENT_TEMPLATE
                        .Replace("${MemberType}", info.type)
                        .Replace("${memberName}", info.name)
                        .Replace("${MemberName}", info.name.UppercaseFirst())
                       )
                .ToArray();

            return string.Join("\n", assignments);
        }
    }
}
