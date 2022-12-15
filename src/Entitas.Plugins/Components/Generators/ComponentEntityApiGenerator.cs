using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.Plugins
{
    public class ComponentEntityApiGenerator : AbstractGenerator
    {
        public override string Name => "Component (Entity API)";

        const string StandardTemplate =
            @"public partial class ${Context.Entity.Type}
{
    public ${Component.Type} ${Component.Name} => (${ComponentType})GetComponent(${Index});
    public bool Has${ComponentName} => HasComponent(${Index});

    public ${Context.Entity.Type} Add${Component.Name}(${newMethodParameters})
    {
        var index = ${Index};
        var component = (${Component.Type})CreateComponent(index, typeof(${Component.Type}));
${memberAssignmentList}
        AddComponent(index, component);
        return this;
    }

    public ${Context.Entity.Type} Replace${ComponentName}(${newMethodParameters})
    {
        var index = ${Index};
        var component = (${Component.Type})CreateComponent(index, typeof(${Component.Type}));
${memberAssignmentList}
        ReplaceComponent(index, component);
        return this;
    }

    public ${Context.Entity.Type} Remove${ComponentName}()
    {
        RemoveComponent(${Index});
        return this;
    }
}
";

        const string FlagTemplate =
            @"public partial class ${Context.Entity.Type}
{
    static readonly ${Component.Type} ${Component.Name}Component = new ${Component.Type}();

    public bool ${prefixedComponentName}
    {
        get => HasComponent(${Index});
        set
        {
            if (value != ${PrefixedComponentName})
            {
                const int index = ${Index};
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : ${Component.Name}Component;

                    AddComponent(index, component);
                }
                else
                {
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
                Path.Combine(context, "Components", $"{context + data.Name.AddComponentSuffix()}.cs"),
                data.ReplacePlaceholders(template)
                    .Replace("${memberAssignmentList}", GetMemberAssignmentList(memberData))
                    .Replace(data, context),
                GetType().FullName
            );
        }

        string GetMemberAssignmentList(MemberData[] memberData) => string.Join("\n", memberData
            .Select(info => $"        component.{info.Name} = new{info.Name.ToUpperFirst()};"));
    }
}
