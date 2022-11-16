using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentEntityApiGenerator : AbstractGenerator
    {
        public override string Name => "Component (Entity API)";

        const string StandardTemplate =
            @"public partial class ${EntityType} {

    public ${ComponentType} ${validComponentName} { get { return (${ComponentType})GetComponent(${Index}); } }
    public bool has${ComponentName} { get { return HasComponent(${Index}); } }

    public void Add${ComponentName}(${newMethodParameters}) {
        var index = ${Index};
        var component = (${ComponentType})CreateComponent(index, typeof(${ComponentType}));
${memberAssignmentList}
        AddComponent(index, component);
    }

    public void Replace${ComponentName}(${newMethodParameters}) {
        var index = ${Index};
        var component = (${ComponentType})CreateComponent(index, typeof(${ComponentType}));
${memberAssignmentList}
        ReplaceComponent(index, component);
    }

    public void Remove${ComponentName}() {
        RemoveComponent(${Index});
    }
}
";

        const string FlagTemplate =
            @"public partial class ${EntityType} {

    static readonly ${ComponentType} ${componentName}Component = new ${ComponentType}();

    public bool ${prefixedComponentName} {
        get { return HasComponent(${Index}); }
        set {
            if (value != ${prefixedComponentName}) {
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

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.Generates)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data
            .Contexts.Select(context => Generate(context, data));

        CodeGenFile Generate(string context, ComponentData data)
        {
            var memberData = data.MemberData;
            var template = memberData.Length == 0
                ? FlagTemplate
                : StandardTemplate;

            return new CodeGenFile(
                Path.Combine(context, "Components", $"{data.ComponentNameWithContext(context).AddComponentSuffix()}.cs"),
                template
                    .Replace("${memberAssignmentList}", GetMemberAssignmentList(memberData))
                    .Replace(data, context),
                GetType().FullName
            );
        }

        string GetMemberAssignmentList(MemberData[] memberData) => string.Join("\n", memberData
            .Select(info => $"        component.{info.Name} = new{info.Name.ToUpperFirst()};"));
    }
}
