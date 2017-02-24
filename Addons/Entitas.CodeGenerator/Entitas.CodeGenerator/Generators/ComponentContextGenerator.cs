using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ComponentContextGenerator : ICodeGenerator {

        public string name { get { return "Component (Context API)"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string STANDARD_COMPONENT_TEMPLATE =
@"public partial class ${Context}Context {

    public ${Context}Entity ${name}Entity { get { return GetGroup(${Context}Matcher.${Name}).GetSingleEntity(); } }
    public ${Type} ${name} { get { return ${name}Entity.${name}; } }
    public bool has${Name} { get { return ${name}Entity != null; } }

    public ${Context}Entity Set${Name}(${memberArgs}) {
        if(has${Name}) {
            throw new Entitas.EntitasException(""Could not set ${name}!\n"" + this + "" already has an entity with ${FullName}!"",
                ""You should check if the context already has a ${name}Entity before setting it or use context.Replace${Name}()."");
        }
        var entity = CreateEntity();
        entity.Add${Name}(${methodArgs});
        return entity;
    }

    public void Replace${Name}(${memberArgs}) {
        var entity = ${name}Entity;
        if(entity == null) {
            entity = Set${Name}(${methodArgs});
        } else {
            entity.Replace${Name}(${methodArgs});
        }
    }

    public void Remove${Name}() {
        DestroyEntity(${name}Entity);
    }
}
";

        const string MEMBER_ARGS_TEMPLATE =
@"${MemberType} new${MemberName}";

        const string METHOD_ARGS_TEMPLATE =
@"new${MemberName}";

        const string FLAG_COMPONENT_TEMPLATE =
@"public partial class ${Context}Context {

    public ${Context}Entity ${name}Entity { get { return GetGroup(${Context}Matcher.${Name}).GetSingleEntity(); } }

    public bool ${prefixedName} {
        get { return ${name}Entity != null; }
        set {
            var entity = ${name}Entity;
            if(value != (entity != null)) {
                if(value) {
                    CreateEntity().${prefixedName} = true;
                } else {
                    DestroyEntity(entity);
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
                .Where(d => d.IsUnique())
                .SelectMany(d => generateExtensions(d))
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                       .Select(contextName => generateExtension(contextName, data))
                       .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var memberData = data.GetMemberData();
            var template = memberData.Length == 0
                                      ? FLAG_COMPONENT_TEMPLATE
                                      : STANDARD_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetComponentName())
                .Replace("${name}", data.GetComponentName().LowercaseFirst())
                .Replace("${FullName}", data.GetFullComponentName())
                .Replace("${prefixedName}", data.GetUniqueComponentPrefix().LowercaseFirst() + data.GetComponentName())
                .Replace("${Type}", data.GetFullTypeName())
                .Replace("${memberArgs}", getMemberArgs(memberData))
                .Replace("${methodArgs}", getMethodArgs(memberData));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + data.GetFullComponentName() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => MEMBER_ARGS_TEMPLATE
                        .Replace("${MemberType}", info.type)
                        .Replace("${MemberName}", info.name.UppercaseFirst()))
                .ToArray();

            return string.Join(", ", args);
        }

        string getMethodArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => METHOD_ARGS_TEMPLATE.Replace("${MemberName}", info.name.UppercaseFirst()))
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
