using System;
using System.Collections.Generic;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;

namespace Entitas.Plugins
{
    public class EntityIndexGenerator : ICodeGenerator
    {
        public string Name => "Entity Index";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string ClassTemplate =
            @"public partial class Contexts {

${indexConstants}

    [Entitas.Plugins.Attributes.PostConstructor]
    public void InitializeEntityIndexes() {
${addIndexes}
    }
}

public static class ContextsExtensions {

${getIndexes}
}";

        const string IndexConstantsTemplate = @"    public const string ${IndexName} = ""${IndexName}"";";

        const string AddIndexTemplate =
            @"        ${context}.AddEntityIndex(new ${IndexType}<${Context}Entity, ${KeyType}>(
            ${IndexName},
            ${context}.GetGroup(${Context}Matcher.${Matcher}),
            (e, c) => ((${ComponentType})c).${MemberName}));";

        const string AddCustomIndexTemplate =
            @"        ${context}.AddEntityIndex(new ${IndexType}(${context}));";

        const string GetIndexTemplate =
            @"    public static System.Collections.Generic.HashSet<${Context}Entity> GetEntitiesWith${IndexName}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntities(${MemberName});
    }";

        const string GetPrimaryIndexTemplate =
            @"    public static ${Context}Entity GetEntityWith${IndexName}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntity(${MemberName});
    }";

        const string CustomMethodTemplate =
            @"    public static ${ReturnType} ${MethodName}(this ${Context}Context context, ${methodArgs}) {
        return ((${IndexType})(context.GetEntityIndex(Contexts.${IndexName}))).${MethodName}(${args});
    }
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var entityIndexData = data
                .OfType<EntityIndexData>()
                .OrderBy(d => d.Name)
                .ToArray();

            return entityIndexData.Length == 0
                ? Array.Empty<CodeGenFile>()
                : GenerateEntityIndexes(entityIndexData);
        }

        CodeGenFile[] GenerateEntityIndexes(EntityIndexData[] data)
        {
            var indexConstants = string.Join("\n", data
                .Select(d => IndexConstantsTemplate
                    .Replace("${IndexName}", d.HasMultiple
                        ? d.Name + d.MemberName.ToUpperFirst()
                        : d.Name)));

            var addIndexes = string.Join("\n\n", data
                .Select(GenerateAddMethods));

            var getIndexes = string.Join("\n\n", data
                .Select(GenerateGetMethods));

            var fileContent = ClassTemplate
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndexes}", addIndexes)
                .Replace("${getIndexes}", getIndexes);

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    fileContent,
                    GetType().FullName
                )
            };
        }

        string GenerateAddMethods(EntityIndexData data) => string.Join("\n", data.Contexts
            .Aggregate(new List<string>(), (addMethods, context) =>
            {
                addMethods.Add(GenerateAddMethod(data, context));
                return addMethods;
            }));

        string GenerateAddMethod(EntityIndexData data, string context) => data.IsCustom
            ? GenerateCustomMethods(data)
            : GenerateMethods(data, context);

        string GenerateCustomMethods(EntityIndexData data) => AddCustomIndexTemplate
            .Replace("${context}", data.Contexts[0].ToLowerFirst())
            .Replace("${IndexType}", data.Type);

        string GenerateMethods(EntityIndexData data, string context)
        {
            var name = data.Name;
            var memberName = data.MemberName;
            var componentType = data.ComponentType;
            return AddIndexTemplate
                .Replace("${context}", context.ToLowerFirst())
                .Replace("${Context}", context)
                .Replace("${IndexName}", data.HasMultiple
                    ? name + memberName.ToUpperFirst()
                    : name)
                .Replace("${Matcher}", name)
                .Replace("${IndexType}", data.Type)
                .Replace("${KeyType}", data.KeyType)
                .Replace("${ComponentType}", componentType)
                .Replace("${MemberName}", memberName)
                .Replace("${componentName}", componentType.ToValidLowerFirst());
        }

        string GenerateGetMethods(EntityIndexData data) => string.Join("\n\n", data.Contexts
            .Aggregate(new List<string>(), (getMethods, context) =>
            {
                getMethods.Add(GenerateGetMethod(data, context));
                return getMethods;
            }));

        string GenerateGetMethod(EntityIndexData data, string context)
        {
            string template;
            switch (data.Type)
            {
                case "Entitas.EntityIndex":
                    template = GetIndexTemplate;
                    break;
                case "Entitas.PrimaryEntityIndex":
                    template = GetPrimaryIndexTemplate;
                    break;
                default:
                    return GetCustomMethods(data);
            }

            var memberName = data.MemberName;
            return template
                .Replace("${Context}", context)
                .Replace("${IndexName}", data.HasMultiple
                    ? data.Name + memberName.ToUpperFirst()
                    : data.Name)
                .Replace("${IndexType}", data.Type)
                .Replace("${KeyType}", data.KeyType)
                .Replace("${MemberName}", memberName);
        }

        string GetCustomMethods(EntityIndexData data) => string.Join("\n", data.CustomMethods
            .Select(m => CustomMethodTemplate
                .Replace("${ReturnType}", m.ReturnType)
                .Replace("${MethodName}", m.MethodName)
                .Replace("${Context}", data.Contexts[0])
                .Replace("${methodArgs}", string.Join(", ", m.Parameters.Select(p => $"{p.Type} {p.Name}")))
                .Replace("${IndexType}", data.Type)
                .Replace("${IndexName}", data.HasMultiple
                    ? data.Name + data.MemberName.ToUpperFirst()
                    : data.Name)
                .Replace("${args}", string.Join(", ", m.Parameters.Select(p => p.Name).ToArray()))));
    }
}
