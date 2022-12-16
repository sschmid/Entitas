using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentContextApiGenerator : ICodeGenerator
    {
        public string Name => "Component (Context API)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string StandardTemplate =
            @"public partial class ${Context.Type}
{
    public ${Context.Entity.Type} ${Component.Name}Entity => GetGroup(${Context.Matcher.Type}.${Component.Name}).GetSingleEntity();
    public ${Component.Type} ${Component.Name} => ${Component.Name}Entity.${Component.Name};
    public bool Has${Component.Name} => ${Component.Name}Entity != null;

    public ${Context.Entity.Type} Set${Component.Name}(${newMethodParameters})
    {
        if (Has${Component.Name})
            throw new Entitas.EntitasException($""Could not set ${Component.Name}!\n{this} already has an entity with ${Component.Type}!"",
                ""You should check if the context already has a ${Component.Name}Entity before setting it or use context.Replace${Component.Name}()."");
        var entity = CreateEntity();
        entity.Add${Component.Name}(${newMethodArgs});
        return entity;
    }

    public ${Context.Entity.Type} Replace${Component.Name}(${newMethodParameters})
    {
        var entity = ${Component.Name}Entity;
        if (entity == null)
            entity = Set${Component.Name}(${newMethodArgs});
        else
            entity.Replace${Component.Name}(${newMethodArgs});
        return entity;
    }

    public void Remove${Component.Name}() => ${Component.Name}Entity.Destroy();
}
";

        const string FlagTemplate =
            @"public partial class ${Context.Type}
{
    public ${Context.Entity.Type} ${Component.Name}Entity => GetGroup(${Context.Matcher.Type}.${Component.Name}).GetSingleEntity();

    public bool ${PrefixedComponentName}
    {
        get => ${Component.Name}Entity != null;
        set
        {
            var entity = ${Component.Name}Entity;
            if (value != (entity != null))
            {
                if (value)
                    CreateEntity().${PrefixedComponentName} = true;
                else
                    entity.Destroy();
            }
        }
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.Generates)
            .Where(d => d.IsUnique)
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ComponentData data)
        {
            var template = data.MemberData.Length == 0
                ? FlagTemplate
                : StandardTemplate;

            return new CodeGenFile(
                Path.Combine(data.Context, "Components", $"{data.Context + data.Name.AddComponentSuffix()}.cs"),
                data.ReplacePlaceholders(template.Replace(data, data.Context)),
                GetType().FullName
            );
        }
    }
}
