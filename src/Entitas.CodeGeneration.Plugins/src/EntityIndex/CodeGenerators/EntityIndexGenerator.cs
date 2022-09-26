using System;
using System.Collections.Generic;
using System.Linq;
using Jenny;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class EntityIndexGenerator : ICodeGenerator, IConfigurable
    {
        public string Name => "Entity Index";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties => _ignoreNamespacesConfig.DefaultProperties;

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string CLASS_TEMPLATE =
            @"public partial class Contexts {

${indexConstants}

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices() {
${addIndices}
    }
}

public static class ContextsExtensions {

${getIndices}
}";

        const string INDEX_CONSTANTS_TEMPLATE = @"    public const string ${IndexName} = ""${IndexName}"";";

        const string ADD_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}<${ContextName}Entity, ${KeyType}>(
            ${IndexName},
            ${contextName}.GetGroup(${ContextName}Matcher.${Matcher}),
            (e, c) => ((${ComponentType})c).${MemberName}));";

        const string ADD_CUSTOM_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}(${contextName}));";

        const string GET_INDEX_TEMPLATE =
            @"    public static System.Collections.Generic.HashSet<${ContextName}Entity> GetEntitiesWith${IndexName}(this ${ContextName}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE =
            @"    public static ${ContextName}Entity GetEntityWith${IndexName}(this ${ContextName}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${IndexName})).GetEntity(${MemberName});
    }";

        const string CUSTOM_METHOD_TEMPLATE =
            @"    public static ${ReturnType} ${MethodName}(this ${ContextName}Context context, ${methodArgs}) {
        return ((${IndexType})(context.GetEntityIndex(Contexts.${IndexName}))).${MethodName}(${args});
    }
";

        public void Configure(Preferences preferences)
        {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var entityIndexData = data
                .OfType<EntityIndexData>()
                .OrderBy(d => d.GetEntityIndexName())
                .ToArray();

            return entityIndexData.Length == 0
                ? Array.Empty<CodeGenFile>()
                : generateEntityIndices(entityIndexData);
        }

        CodeGenFile[] generateEntityIndices(EntityIndexData[] data)
        {
            var indexConstants = string.Join("\n", data
                .Select(d => INDEX_CONSTANTS_TEMPLATE
                    .Replace("${IndexName}", d.GetHasMultiple()
                        ? d.GetEntityIndexName() + d.GetMemberName().ToUpperFirst()
                        : d.GetEntityIndexName())));

            var addIndices = string.Join("\n\n", data
                .Select(generateAddMethods));

            var getIndices = string.Join("\n\n", data
                .Select(generateGetMethods));

            var fileContent = CLASS_TEMPLATE
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndices}", addIndices)
                .Replace("${getIndices}", getIndices);

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    fileContent,
                    GetType().FullName
                )
            };
        }

        string generateAddMethods(EntityIndexData data) => string.Join("\n", data.GetContextNames()
            .Aggregate(new List<string>(), (addMethods, contextName) =>
            {
                addMethods.Add(generateAddMethod(data, contextName));
                return addMethods;
            }));

        string generateAddMethod(EntityIndexData data, string contextName) => data.IsCustom()
            ? generateCustomMethods(data)
            : generateMethods(data, contextName);

        string generateCustomMethods(EntityIndexData data) => ADD_CUSTOM_INDEX_TEMPLATE
            .Replace("${contextName}", data.GetContextNames()[0].ToLowerFirst())
            .Replace("${IndexType}", data.GetEntityIndexType());

        string generateMethods(EntityIndexData data, string contextName) => ADD_INDEX_TEMPLATE
            .Replace("${contextName}", contextName.ToLowerFirst())
            .Replace("${ContextName}", contextName)
            .Replace("${IndexName}", data.GetHasMultiple()
                ? data.GetEntityIndexName() + data.GetMemberName().ToUpperFirst()
                : data.GetEntityIndexName())
            .Replace("${Matcher}", data.GetEntityIndexName())
            .Replace("${IndexType}", data.GetEntityIndexType())
            .Replace("${KeyType}", data.GetKeyType())
            .Replace("${ComponentType}", data.GetComponentType())
            .Replace("${MemberName}", data.GetMemberName())
            .Replace("${componentName}", data.GetComponentType()
                .ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces)
                .ToLowerFirst()
                .AddPrefixIfIsKeyword());

        string generateGetMethods(EntityIndexData data) => string.Join("\n\n", data.GetContextNames()
            .Aggregate(new List<string>(), (getMethods, contextName) =>
            {
                getMethods.Add(generateGetMethod(data, contextName));
                return getMethods;
            }));

        string generateGetMethod(EntityIndexData data, string contextName)
        {
            string template;
            if (data.GetEntityIndexType() == "Entitas.EntityIndex")
            {
                template = GET_INDEX_TEMPLATE;
            }
            else if (data.GetEntityIndexType() == "Entitas.PrimaryEntityIndex")
            {
                template = GET_PRIMARY_INDEX_TEMPLATE;
            }
            else
            {
                return getCustomMethods(data);
            }

            return template
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetHasMultiple()
                    ? data.GetEntityIndexName() + data.GetMemberName().ToUpperFirst()
                    : data.GetEntityIndexName())
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${MemberName}", data.GetMemberName());
        }

        string getCustomMethods(EntityIndexData data) => string.Join("\n", data.GetCustomMethods()
            .Select(m => CUSTOM_METHOD_TEMPLATE
                .Replace("${ReturnType}", m.returnType)
                .Replace("${MethodName}", m.methodName)
                .Replace("${ContextName}", data.GetContextNames()[0])
                .Replace("${methodArgs}", string.Join(", ", m.parameters.Select(p => $"{p.type} {p.name}")))
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${IndexName}", data.GetHasMultiple()
                    ? data.GetEntityIndexName() + data.GetMemberName().ToUpperFirst()
                    : data.GetEntityIndexName())
                .Replace("${args}", string.Join(", ", m.parameters.Select(p => p.name).ToArray()))));
    }
}
