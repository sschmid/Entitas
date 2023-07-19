using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void EntityIndexExtension(SourceProductionContext spc, ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (components.Length == 0)
                return;

            if (!EntitasAnalyzerConfigOptions.ComponentEntityIndexExtension(optionsProvider, method.SyntaxTree))
                return;

            var componentMemberPairs = components
                .SelectMany(component => component.Members
                    .Where(member => member.EntityIndexType != -1)
                    .Select(member => (Component: component, Member: member)))
                .ToImmutableArray();

            spc.AddSource(
                GeneratedPath($"{method.ContextFullName}EntityIndexExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EntityIndexExtension))) +
                $"using global::{method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.ContextNamespace,
                    $$"""
                    public static class {{method.ContextName}}EntityIndexExtension
                    {
                    {{EntityIndexNames(componentMemberPairs)}}

                        public static {{method.ContextName}} AddEntityIndexes(this {{method.ContextName}} context)
                        {
                    {{AddEntityIndexes(componentMemberPairs, method.ContextFullName)}}
                            return context;
                        }
                    }

                    """) + '\n' +
                EntityIndexExtensionMethods(componentMemberPairs, method));

            static string EntityIndexNames(ImmutableArray<(ComponentDeclaration Component, MemberDeclaration Member)> pairs)
            {
                return string.Join("\n", pairs.Select(static pair =>
                {
                    var indexName = $"{pair.Component.FullPrefix}{pair.Member.Name}";
                    return $"    public const string {indexName} = \"{indexName}\";";
                }));
            }

            static string AddEntityIndexes(ImmutableArray<(ComponentDeclaration Component, MemberDeclaration Member)> pairs, string context)
            {
                return string.Join("\n", pairs.Select(pair =>
                {
                    var indexName = $"{pair.Component.FullPrefix}{pair.Member.Name}";
                    var indexType = pair.Member.EntityIndexType == 0 ? "EntityIndex" : "PrimaryEntityIndex";
                    var contextPrefix = ContextPrefix(context);
                    var contextAwareComponentPrefix = pair.Component.ContextAwareComponentPrefix(contextPrefix);
                    return $$"""
                                    context.AddEntityIndex(new global::Entitas.{{indexType}}<Entity, {{pair.Member.Type}}>(
                                        {{indexName}},
                                        context.GetGroup(global::{{CombinedNamespace(pair.Component.Namespace, contextAwareComponentPrefix)}}Matcher.{{pair.Component.Prefix}}),
                                        (entity, component) => ((global::{{pair.Component.FullName}})component).{{pair.Member.Name}}));

                            """;
                }));
            }

            static string EntityIndexExtensionMethods(ImmutableArray<(ComponentDeclaration Component, MemberDeclaration Member)> pairs, ContextInitializationMethodDeclaration method) =>
                string.Join("\n", pairs
                    .GroupBy(pair => pair.Component.Namespace)
                    .Select(group => NamespaceDeclaration(group.Key,
                        $$"""
                        public static class EntityIndexExtension
                        {
                        {{string.Join("\n\n", group.Select(pair => pair.Member.EntityIndexType == 0
                            ? $$"""
                                    public static global::System.Collections.Generic.HashSet<Entity> GetEntitiesWith{{pair.Component.Prefix}}{{pair.Member.Name}}(this global::{{method.ContextFullName}} context, {{pair.Member.Type}} {{pair.Member.ValidLowerFirstName}})
                                    {
                                        return ((global::Entitas.EntityIndex<Entity, {{pair.Member.Type}}>)context.GetEntityIndex(global::{{method.ContextFullName}}EntityIndexExtension.{{pair.Component.FullPrefix}}{{pair.Member.Name}})).GetEntities({{pair.Member.ValidLowerFirstName}});
                                    }
                                """
                            : $$"""
                                    public static Entity GetEntityWith{{pair.Component.Prefix}}{{pair.Member.Name}}(this global::{{method.ContextFullName}} context, {{pair.Member.Type}} {{pair.Member.ValidLowerFirstName}})
                                    {
                                        return ((global::Entitas.PrimaryEntityIndex<Entity, {{pair.Member.Type}}>)context.GetEntityIndex(global::{{method.ContextFullName}}EntityIndexExtension.{{pair.Component.FullPrefix}}{{pair.Member.Name}})).GetEntity({{pair.Member.ValidLowerFirstName}});
                                    }
                                """))}}
                        }

                        """
                    )));
        }
    }
}
