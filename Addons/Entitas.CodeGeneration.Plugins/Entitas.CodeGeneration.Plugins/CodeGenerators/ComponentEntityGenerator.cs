using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentEntityGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Component (Entity API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

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
            if (value != ${prefixedName}) {
                var index = ${Index};
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : ${componentName}Component;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .SelectMany(generateExtensions)
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateExtension(contextName, data))
                .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var index = contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP + "." + componentName;
            var memberData = data.GetMemberData();
            var template = memberData.Length == 0
                ? FLAG_COMPONENT_TEMPLATE
                : STANDARD_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedName}", data.GetUniquePrefix().LowercaseFirst() + componentName)
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
