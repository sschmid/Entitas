using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentContextGenerator : ICodeGenerator {

        const string normalComponentTemplate =
@"using Entitas;
using Entitas.Api;

public sealed partial class ${Context}Context : Context<GameEntity> {

    public ${Context}Entity ${name}Entity { get { return GetGroup(${Context}Matcher.${Name}).GetSingleEntity(); } }
    public ${Name}Component ${name} { get { return ${name}Entity.${name}; } }
    public bool has${Name} { get { return ${name}Entity != null; } }

    public ${Context}Entity Set${Name}(${memberArgs}) {
        if(has${Name}) {
            throw new EntitasException(""Could not set ${name}!\n"" + this + "" already has an entity with ${Name}Component!"",
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

        const string memberArgsTemplate =
@"${MemberType} new${MemberName}";

        const string methodArgsTemplate =
@"new${MemberName}";

        const string flagComponentTemplate =
@"public sealed partial class ${Context}Context {

    public ${Context}Entity ${name}Entity { get { return GetGroup(${Context}Matcher.${Name}).GetSingleEntity(); } }

    public bool is${Name} {
        get { return ${name}Entity != null; }
        set {
            var entity = ${name}Entity;
            if(value != (entity != null)) {
                if(value) {
                    CreateEntity().is${Name} = true;
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
            var memberInfos = data.GetMemberInfos();

            var template = memberInfos.Count == 0
                                      ? flagComponentTemplate
                                      : normalComponentTemplate;

            var fileContent = template
                .Replace("${Context}", contextName)
                .Replace("${Name}", shortComponentName)
                .Replace("${name}", shortComponentName.LowercaseFirst())
                .Replace("${memberArgs}", getMemberArgs(memberInfos))
                .Replace("${methodArgs}", getMethodArgs(memberInfos));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + "Components" +
                Path.DirectorySeparatorChar + "Unique" + contextName + fullComponentName + ".cs",
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

        string getMethodArgs(List<PublicMemberInfo> memberInfos) {
            var args = memberInfos
                .Select(info => methodArgsTemplate.Replace("${MemberName}", info.name.UppercaseFirst()))
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
