using System.Linq;

namespace Entitas.CodeGenerator {

    public class EntityIndexGenerator : ICodeGenerator {

        public string name { get { return "Entity Index"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string CLASS_TEMPLATE =
@"public partial class Contexts {

${indexConstants}

    public void InitEntityIndices() {
${addIndices}
    }
}

public static class ContextsExtensions {

${getIndices}
}";

        const string INDEX_CONSTANTS_TEMPLATE = @"    public const string ${Name} = ""${Name}"";";

        const string ADD_INDEX_TEMPLATE =
@"        ${context}.AddEntityIndex(${Name}, new Entitas.${IndexType}<${Context}Entity, ${KeyType}>(
            ${context}.GetGroup(${Context}Matcher.${Name}),
            (e, c) => { var component = c as ${ComponentType}; return component != null ? component.${MemberName} : e.${componentName}.${MemberName}; }));";

        const string GET_INDEX_TEMPLATE =
@"    public static System.Collections.Generic.HashSet<${Context}Entity> GetEntitiesWith${Name}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((Entitas.${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${Name})).GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE =
@"    public static bool HasEntityWith${Name}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((Entitas.${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${Name})).HasEntity(${MemberName});
    }

    public static ${Context}Entity GetEntityWith${Name}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((Entitas.${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${Name})).GetEntity(${MemberName});
    }";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return generateEntityIndices(data.OfType<EntityIndexData>().ToArray());
        }

        CodeGenFile[] generateEntityIndices(EntityIndexData[] data) {
            var indexConstants = string.Join("\n", data
                                        .Select(d => INDEX_CONSTANTS_TEMPLATE.Replace("${Name}", d.GetEntityIndexName()))
                                        .ToArray());

            var addIndices = string.Join("\n\n", data
                                         .Select(d => ADD_INDEX_TEMPLATE
                                                 .Replace("${context}", d.GetContextNames()[0].LowercaseFirst())
                                                 .Replace("${Context}", d.GetContextNames()[0])
                                                 .Replace("${Name}", d.GetEntityIndexName())
                                                 .Replace("${componentName}", d.GetComponentName().LowercaseFirst())
                                                 .Replace("${IndexType}", d.GetEntityIndexType())
                                                 .Replace("${KeyType}", d.GetKeyType())
                                                 .Replace("${ComponentType}", d.GetComponentType())
                                                 .Replace("${MemberName}", d.GetMemberName()))
                                         .ToArray());

            var getIndices = string.Join("\n\n", data
                                         .Select(d => (d.IsPrimary() ? GET_PRIMARY_INDEX_TEMPLATE : GET_INDEX_TEMPLATE)
                                                 .Replace("${Context}", d.GetContextNames()[0])
                                                 .Replace("${Name}", d.GetEntityIndexName())
                                                 .Replace("${IndexType}", d.GetEntityIndexType())
                                                 .Replace("${KeyType}", d.GetKeyType())
                                                 .Replace("${MemberName}", d.GetMemberName()))
                                         .ToArray());

            var fileContent = CLASS_TEMPLATE
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndices}", addIndices)
                .Replace("${getIndices}", getIndices);

            return new [] { new CodeGenFile(
                "EntityIndexContextExtension.cs",
                fileContent,
                GetType().FullName
            ) };
        }
    }
}
