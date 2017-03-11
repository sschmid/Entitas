using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class EntityIndexGenerator : ICodeGenerator {

        public string name { get { return "Entity Index"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string CLASS_TEMPLATE =
@"public partial class Contexts {

${indexConstants}

    [Entitas.CodeGenerator.Api.PostConstructor]
    public void InitializeEntityIndices() {
${addIndices}
    }
}

public static class ContextsExtensions {

${getIndices}
}";

        const string INDEX_CONSTANTS_TEMPLATE = @"    public const string ${Name} = ""${Name}"";";

        const string ADD_INDEX_TEMPLATE =
@"        ${context}.AddEntityIndex(new ${IndexType}<${Context}Entity, ${KeyType}>(
            ""${Name}""
            ${context}.GetGroup(${Context}Matcher.${Name}),
            (e, c) => { var component = c as ${ComponentType}; return component != null ? component.${MemberName} : e.${componentName}.${MemberName}; }));";

        const string ADD_CUSTOM_INDEX_TEMPLATE =
@"        ${context}.AddEntityIndex(new ${IndexType}(${context}));";

        const string GET_INDEX_TEMPLATE =
@"    public static System.Collections.Generic.HashSet<${Context}Entity> GetEntitiesWith${Name}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${Name})).GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE =
@"    public static ${Context}Entity GetEntityWith${Name}(this ${Context}Context context, ${KeyType} ${MemberName}) {
        return ((${IndexType}<${Context}Entity, ${KeyType}>)context.GetEntityIndex(Contexts.${Name})).GetEntity(${MemberName});
    }";

        const string CUSTOM_METHOD_TEMPLATE =
@"    public static ${ReturnType} ${MethodName}(this ${Context}Context context, ${methodArgs}) {
        return ((${IndexType})(context.GetEntityIndex(Contexts.${Name}))).${MethodName}(${args});
    }
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var entityIndexData = data.OfType<EntityIndexData>().ToArray();
            return entityIndexData.Length == 0
                                  ? new CodeGenFile[0]
                                  : generateEntityIndices(entityIndexData);
        }

        CodeGenFile[] generateEntityIndices(EntityIndexData[] data) {

            var indexConstants = string.Join("\n", data
                                        .Select(d => INDEX_CONSTANTS_TEMPLATE.Replace("${Name}", d.GetEntityIndexName()))
                                        .ToArray());

            var addIndices = string.Join("\n\n", data
                                         .Select(d => generateAddMethods(d))
                                         .ToArray());

            var getIndices = string.Join("\n\n", data
                                         .Select(d => generateGetMethods(d))
                                         .ToArray());

            var fileContent = CLASS_TEMPLATE
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndices}", addIndices)
                .Replace("${getIndices}", getIndices);

            return new[] { new CodeGenFile(
                "ContextsEntityIndices.cs",
                fileContent,
                GetType().FullName
            ) };
        }

        string generateAddMethods(EntityIndexData data) {
            return string.Join("\n", data.GetContextNames()
                       .Aggregate(new List<string>(), (addMethods, contextName) => {
                           addMethods.Add(generateAddMethod(data, contextName));
                           return addMethods;
                       }).ToArray());
        }

        string generateAddMethod(EntityIndexData data, string contextName) {
            return data.IsCustom()
                       ? generateCustomMethods(data)
                       : generateMethods(data, contextName);
        }

        string generateCustomMethods(EntityIndexData data) {
            return ADD_CUSTOM_INDEX_TEMPLATE
                .Replace("${context}", data.GetContextNames()[0].LowercaseFirst())
                .Replace("${IndexType}", data.GetEntityIndexType());
        }

        string generateMethods(EntityIndexData data, string contextName) {
            return ADD_INDEX_TEMPLATE
                .Replace("${context}", contextName.LowercaseFirst())
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetEntityIndexName())
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${ComponentType}", data.GetComponentType())
                .Replace("${MemberName}", data.GetMemberName())
                .Replace("${componentName}", data.GetComponentName().LowercaseFirst());
        }

        string generateGetMethods(EntityIndexData data) {
            return string.Join("\n\n", data.GetContextNames()
                            .Aggregate(new List<string>(), (getMethods, contextName) => {
                                getMethods.Add(generateGetMethod(data, contextName));
                                return getMethods;
                            }).ToArray());
        }

        string generateGetMethod(EntityIndexData data, string contextName) {
            var template = "";
            if(data.GetEntityIndexType() == "Entitas.EntityIndex") {
                template = GET_INDEX_TEMPLATE;
            } else if(data.GetEntityIndexType() == "Entitas.PrimaryEntityIndex") {
                template = GET_PRIMARY_INDEX_TEMPLATE;
            } else {
                return getCustomMethods(data);
            }

            return template
                .Replace("${Context}", contextName)
                .Replace("${Name}", data.GetEntityIndexName())
                .Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${MemberName}", data.GetMemberName());
        }

        string getCustomMethods(EntityIndexData data) {
            return string.Join("\n", data.GetCustomMethods()
                                       .Select(m => CUSTOM_METHOD_TEMPLATE
                    .Replace("${ReturnType}", m.ReturnType.ToCompilableString())
                    .Replace("${MethodName}", m.Name)
                    .Replace("${Context}", data.GetContextNames()[0])
                    .Replace("${methodArgs}", string.Join(", ", m.GetParameters().Select(p => p.ParameterType.ToCompilableString() + " " + p.Name).ToArray()))
                    .Replace("${IndexType}", data.GetEntityIndexType())
                    .Replace("${Name}", data.GetEntityIndexName())
                    .Replace("${args}", string.Join(", ", m.GetParameters().Select(p => p.Name).ToArray()))).ToArray());
        }
   }
}
