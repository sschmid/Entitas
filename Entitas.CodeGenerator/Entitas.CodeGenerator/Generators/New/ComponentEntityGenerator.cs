using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentEntityGenerator : ICodeGenerator {

        const string generatedComponentTemplate =
@"public partial class ${Context}Entity {

    public ${ComponentType} ${name} { get { return (${ComponentType})GetComponent(${Index}); } }
    public bool has${Name} { get { return HasComponent(${Index}); } }

    public void Add${Name}(${Type} newValue) {
        var component = CreateComponent<${ComponentType}>(${Index});
        component.value = newValue;
        AddComponent(${Index}, component);
    }

    public void Replace${Name}(${Type} newValue) {
        var component = CreateComponent<${ComponentType}>(${Index});
        component.value = newValue;
        ReplaceComponent(${Index}, component);
    }

    public void Remove${Name}() {
        RemoveComponent(${Index});
    }
}
";

        const string normalComponentTemplate =
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

        const string memberArgsTemplate =
@"${MemberType} new${MemberName}";

        const string memberAssignmentTemplate =
@"        component.${memberName} = new${MemberName};";

        const string flagComponentTemplate =
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
            return data.ShouldGenerateComponent()
                       ? generateExtensionForGeneratedComponent(contextName, data)
                       : generateExtensionForComponent(contextName, data);
        }

        CodeGenFile generateExtensionForGeneratedComponent(string contextName, ComponentData data) {
            var fullComponentName = data.GetShortTypeName().AddComponentSuffix();
            var shortComponentName = fullComponentName.RemoveComponentSuffix();
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + shortComponentName.ToUpper();

            var fileContent = generatedComponentTemplate
                .Replace("${Context}", contextName)
                .Replace("${Name}", shortComponentName)
                .Replace("${name}", shortComponentName.LowercaseFirst())
                .Replace("${ComponentType}", fullComponentName)
                .Replace("${Type}", data.GetFullTypeName())
                .Replace("${Index}", index);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + "Components" +
                Path.DirectorySeparatorChar + contextName + fullComponentName + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        CodeGenFile generateExtensionForComponent(string contextName, ComponentData data) {
            var fullComponentName = data.GetShortTypeName();
            var shortComponentName = fullComponentName.RemoveComponentSuffix();
            var memberInfos = data.GetMemberInfos();
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + shortComponentName.ToUpper();

            var template = memberInfos.Count == 0
                                      ? flagComponentTemplate
                                      : normalComponentTemplate;

            var fileContent = template
                .Replace("${Context}", contextName)
                .Replace("${Name}", shortComponentName)
                .Replace("${name}", shortComponentName.LowercaseFirst())
                .Replace("${prefixedName}", data.GetUniqueComponentPrefix().LowercaseFirst() + shortComponentName)
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
