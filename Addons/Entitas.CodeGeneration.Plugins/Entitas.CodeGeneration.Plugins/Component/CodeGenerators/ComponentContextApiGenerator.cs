using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentContextApiGenerator : AbstractGenerator
    {
        public override string name
        {
            get { return "Component Context Extension"; }
        }

        const string STANDARD_TEMPLATE = @"public static class ${ComponentName}ContextExtension
{
    public static ${ContextName}Entity Get${ComponentName}Entity(this ${ContextName}Context context)
    {
        return context.GetGroup(${ComponentName}Matcher.Instance).GetSingleEntity();
    }

    public static ${ComponentType} Get${ComponentName}(this ${ContextName}Context context)
    {
        return context.Get${ComponentName}Entity().Get${ComponentName}();
    }

    public static bool Has${ComponentName}(this ${ContextName}Context context)
    {
        return context.Get${ComponentName}Entity() != null;
    }

    public static ${ContextName}Entity Set${ComponentName}(this ${ContextName}Context context, ${newMethodParameters})
    {
        if (context.Has${ComponentName}())
        {
            throw new Entitas.EntitasException(""Could not set ${ComponentName}!\n"" + context + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${ComponentName} entity before setting it or use context.Replace${ComponentName}()."");
        }

        var entity = context.CreateEntity();
        entity.Add${ComponentName}(${newMethodArgs});
        return entity;
    }

    public static ${ContextName}Entity Replace${ComponentName}(this ${ContextName}Context context, ${newMethodParameters})
    {
        var entity = context.Get${ComponentName}Entity();
        if (entity == null)
            entity = context.Set${ComponentName}(${newMethodArgs});
        else
            entity.Replace${ComponentName}(${newMethodArgs});

        return entity;
    }

    public static void Remove${ComponentName}(this ${ContextName}Context context)
    {
        context.Get${ComponentName}Entity().Destroy();
    }
}
";

        const string FLAG_TEMPLATE = @"public static class ${ComponentName}ContextExtension
{
    public static ${ContextName}Entity Get${ComponentName}Entity(this ${ContextName}Context context)
    {
        return context.GetGroup(${ComponentName}Matcher.Instance).GetSingleEntity();
    }

    public static bool ${PrefixedComponentName}(this ${ContextName}Context context)
    {
        return context.Get${ComponentName}Entity() != null;
    }

    public static void ${PrefixedComponentName}(this ${ContextName}Context context, bool value) {
        var entity = context.Get${ComponentName}Entity();
        if (value != (entity != null))
        {
            if (value)
                context.CreateEntity().${PrefixedComponentName}(true);
            else
                entity.Destroy();
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .Where(d => d.IsUnique())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data)
        {
            return data.GetContextNames()
                .Select(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data)
        {
            var template = data.GetMemberData().Length == 0
                ? FLAG_TEMPLATE
                : STANDARD_TEMPLATE;

            var fileContent = template
                .Replace(data, contextName)
                .WrapInNamespace(data.GetNamespace(), contextName);

            return new CodeGenFile(
                data.GetTypeName().ToFileName(contextName),
                fileContent,
                GetType().FullName
            );
        }
    }
}
