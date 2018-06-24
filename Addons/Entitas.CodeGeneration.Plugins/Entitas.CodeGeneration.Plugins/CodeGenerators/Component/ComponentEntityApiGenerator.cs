using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentEntityApiGenerator : AbstractGenerator {

        public override string name { get { return "Component (Entity API)"; } }

        const string STANDARD_TEMPLATE =
            @"public partial class ${EntityType} {

    public ${ComponentType} ${componentName} { get { return (${ComponentType})GetComponent(${Index}); } }
    public bool has${ComponentName} { get { return HasComponent(${Index}); } }

    public void Add${ComponentName}(${newMethodParameters}) {
        var index = ${Index};
        var component = CreateComponent<${ComponentType}>(index);
${memberAssignmentList}
        AddComponent(index, component);
    }

${helpList}


    public void Replace${ComponentName}(${newMethodParameters}) {
        var index = ${Index};
        var component = CreateComponent<${ComponentType}>(index);
${memberAssignmentList}
        ReplaceComponent(index, component);
    }

    public void Remove${ComponentName}() {
        RemoveComponent(${Index});
    }
}
";

        const string FLAG_TEMPLATE =
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

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data) {
            
            var template = data.GetMemberData().Length == 0
                ? FLAG_TEMPLATE
                : STANDARD_TEMPLATE;

            var fileContent = template
                .Replace("${memberAssignmentList}", getMemberAssignmentList(data.GetMemberData()))
                .Replace("${helpList}", getMemberTypeHelpList(contextName,data.GetMemberData()))
                .Replace(data, contextName);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                data.ComponentNameWithContext(contextName).AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberTypeHelpList(string contextName,MemberData[] memberData)
        {
            return string.Join("\n", memberData
                .Select(info => "    public " + info.type + " ${ComponentType}" + info.name + "{ get{ return ${componentName}" + "." + info.name + ";} set{${componentName}."+ info.name + "=value;} }")
                .ToArray()
            );
        }
        string getMemberAssignmentList(MemberData[] memberData) {
            return string.Join("\n", memberData
                .Select(info => "        component." + info.name + " = new" + info.name.UppercaseFirst() + ";")
                .ToArray()
            );
        }
    }
}
