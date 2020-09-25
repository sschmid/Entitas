using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentEntityApiGenerator : AbstractGenerator
    {
        public override string name
        {
            get { return "Component Entity Extension"; }
        }

        const string STANDARD_TEMPLATE = @"public static class ${ComponentName}EntityExtension
{
    public static ${ComponentType} Get${ComponentName}(this ${ContextName}Entity entity)
    {
        return (${ComponentType})entity.GetComponent(${ComponentName}ComponentIndex.Value);
    }

    public static bool Has${ComponentName}(this ${ContextName}Entity entity)
    {
        return entity.HasComponent(${ComponentName}ComponentIndex.Value);
    }

    public static ${ContextName}Entity Add${ComponentName}(this ${ContextName}Entity entity, ${newMethodParameters})
    {
        var index = ${ComponentName}ComponentIndex.Value;
        var component = entity.CreateComponent<${ComponentType}>(index);
${memberAssignmentList}
        entity.AddComponent(index, component);
        return entity;
    }

    public static ${ContextName}Entity Replace${ComponentName}(this ${ContextName}Entity entity, ${newMethodParameters})
    {
        var index = ${ComponentName}ComponentIndex.Value;
        var component = entity.CreateComponent<${ComponentType}>(index);
${memberAssignmentList}
        entity.ReplaceComponent(index, component);
        return entity;
    }

    public static void Remove${ComponentName}(this ${ContextName}Entity entity)
    {
        entity.RemoveComponent(${ComponentName}ComponentIndex.Value);
    }
}
";

        const string FLAG_TEMPLATE = @"public static class ${ComponentName}EntityExtension
{
    static readonly ${ComponentName}Component _${componentName}Component = new ${ComponentName}Component();

    public static bool ${PrefixedComponentName}(this ${ContextName}Entity entity)
    {
        return entity.HasComponent(${ComponentName}ComponentIndex.Value);
    }

    public static ${ContextName}Entity ${PrefixedComponentName}(this ${ContextName}Entity entity, bool value)
    {
        if (value != entity.${PrefixedComponentName}())
        {
            var index = ${ComponentName}ComponentIndex.Value;
            if (value)
            {
                var componentPool = entity.GetComponentPool(index);
                var component = componentPool.Count > 0
                    ? componentPool.Pop()
                    : _${componentName}Component;

                entity.AddComponent(index, component);
            }
            else
            {
                entity.RemoveComponent(index);
            }
        }

        return entity;
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
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
                .Replace("${memberAssignmentList}", getMemberAssignmentList(data.GetMemberData()))
                .Replace(data, contextName)
                .WrapInNamespace(data.GetNamespace(), contextName);

            return new CodeGenFile(
                data.GetTypeName().ToFileName(contextName),
                fileContent,
                GetType().FullName
            );
        }

        string getMemberAssignmentList(MemberData[] memberData)
        {
            return string.Join("\n", memberData
                .Select(info => "        component." + info.name + " = new" + info.name.UppercaseFirst() + ";")
                .ToArray()
            );
        }
    }
}
