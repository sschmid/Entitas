using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public sealed class ComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var allComponents = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclarations)
                .SelectMany((components, _) => components);

            var fullNameOrContextChanged = allComponents.WithComparer(new FullNameAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrContextChanged, OnFullNameOrContextChanged);

            var fullNameOrMembersOrContextChanged = allComponents.WithComparer(new FullNameAndMembersAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextChanged, OnFullNameOrMembersOrContextChanged);

            var fullNameOrMembersOrContextOrIsUniqueChanged = allComponents.WithComparer(new FullNameAndMembersAndContextAndIsUniqueComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextOrIsUniqueChanged, OnFullNameOrMembersOrContextOrIsUniqueChanged);

            var contextInitializationMethods = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(method => method is not null)
                .Select((method, _) => method!.Value);

            var componentsOrderChanged = fullNameOrContextChanged
                .Collect()
                .Select((components, _) => components
                    .OrderBy(component => component.FullName)
                    .ToImmutableArray());

            var lookupChanged = contextInitializationMethods.Combine(componentsOrderChanged);
            initContext.RegisterSourceOutput(lookupChanged, OnLookupChanged);
        }

        static bool IsComponentCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.BaseList.Types.Any(baseType => baseType.Type switch
                   {
                       IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier is { Text: "IComponent" },
                       QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax is
                       {
                           Left: IdentifierNameSyntax { Identifier.Text: "Entitas" },
                           Right: IdentifierNameSyntax { Identifier.Text: "IComponent" }
                       },
                       _ => false
                   })
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static ImmutableArray<ComponentDeclaration> CreateComponentDeclarations(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return ImmutableArray<ComponentDeclaration>.Empty;

            var interfaceType = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (interfaceType is null)
                return ImmutableArray<ComponentDeclaration>.Empty;

            var isComponent = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
            if (!isComponent)
                return ImmutableArray<ComponentDeclaration>.Empty;

            return symbol.GetAttributes()
                .Where(attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextAttribute")
                .Select(attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString().RemoveSuffix("Context"))
                .Distinct()
                .Select(context => new ComponentDeclaration(symbol, context, cancellationToken))
                .ToImmutableArray();
        }

        static bool IsContextInitializationMethodCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
                   && candidate.ReturnType is PredefinedTypeSyntax predefined
                   && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        static ContextInitializationMethodDeclaration? CreateContextInitializationMethodDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (MethodDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            if (!symbol.ContainingType.IsStatic || symbol.ContainingType.DeclaredAccessibility != Accessibility.Public)
                return null;

            var context = symbol.GetAttributes()
                .Where(attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextInitializationAttribute")
                .Select(attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct()
                .SingleOrDefault();

            if (context is null)
                return null;

            return new ContextInitializationMethodDeclaration(symbol, context);
        }

        static void OnFullNameOrContextChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            ComponentIndex(spc, component);
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component)
        {
            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.Context}.ComponentIndex"),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using global::{component.Context};\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public static class {{component.ContextAwareComponentPrefix}}ComponentIndex
                    {
                        public static ComponentIndex Index;
                    }

                    """));
        }

        static void OnFullNameOrMembersOrContextChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            EntityExtension(spc, component);
        }

        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component)
        {
            var className = $"{component.ContextAwareComponentPrefix}EntityExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                        {
                            var index = Index.Value;
                            var component = ({{component.Name}})entity.CreateComponent(index, typeof({{component.Name}}));
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                        {
                            var index = Index.Value;
                            var component = ({{component.Name}})entity.CreateComponent(index, typeof({{component.Name}}));
                    {{ComponentValueAssignments(component)}}
                            entity.ReplaceComponent(index, component);
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        static readonly {{component.Name}} Single{{component.Name}} = new {{component.Name}}();

                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.AddComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.ReplaceComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.Context}.EntityExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                $"using global::{component.Context};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, component.ContextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static void OnFullNameOrMembersOrContextOrIsUniqueChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            if (component.IsUnique)
                ContextExtension(spc, component);
        }

        static void ContextExtension(SourceProductionContext spc, ComponentDeclaration component)
        {
            var className = $"{component.ContextAwareComponentPrefix}ContextExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{component.Context}}Context context, {{ComponentMethodArgs(component)}})
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{component.Context}}Context context, {{ComponentMethodArgs(component)}})
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            if (entity == null)
                                entity = context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodParams(component)}});
                            else
                                entity.Replace{{component.ComponentPrefix}}({{ComponentMethodParams(component)}});

                            return entity;
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{component.Context}}Context context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{component.Context}}Context context)
                        {
                            return context.GetGroup(Matcher.AllOf(stackalloc[] { Index })).GetSingleEntity();
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{component.Context}}Context context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{component.Context}}Context context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() ?? context.CreateEntity().Add{{component.ComponentPrefix}}();
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{component.Context}}Context context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{component.Context}}Context context)
                        {
                            return context.GetGroup(Matcher.AllOf(stackalloc[] { Index })).GetSingleEntity();
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.Context}.ContextExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextExtension))) +
                DisableNullable() +
                $"using global::{component.Context};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, component.ContextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static string ComponentMethodArgs(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(member => $"{member.Type} {member.ValidLowerFirstName}"));
        }

        static string ComponentMethodParams(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(member => $"{member.ValidLowerFirstName}"));
        }

        static string ComponentValueAssignments(ComponentDeclaration component)
        {
            return string.Join("\n", component.Members.Select(member =>
                $$"""
                        component.{{member.Name}} = {{member.ValidLowerFirstName}};
                """));
        }

        static void OnLookupChanged(SourceProductionContext spc, (ContextInitializationMethodDeclaration Left, ImmutableArray<ComponentDeclaration> Right) pair)
        {
            var (method, components) = pair;
            var componentsForContext = components
                .Where(component => component.Context == method.FullContextPrefix)
                .ToImmutableArray();

            ContextInitializationMethod(spc, method, componentsForContext);
        }

        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
        {
            spc.AddSource(
                GeneratedPath(CombinedNamespace(method.Namespace, $"{method.Name}.ContextInitializationMethod")),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationMethod))) +
                $"using global::{method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.Namespace,
                    $$"""
                    public static partial class {{method.Class}}
                    {
                        public static partial void {{method.Name}}()
                        {
                    {{ComponentIndexAssignments(method, components)}}

                            {{method.Context}}.ComponentNames = new string[]
                            {
                    {{ComponentNames(components)}}
                            };

                            {{method.Context}}.ComponentTypes = new global::System.Type[]
                            {
                    {{ComponentTypes(components)}}
                            };
                        }
                    }

                    """));

            static string ComponentIndexAssignments(ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join("\n", components.Select((component, i) =>
                {
                    var contextPrefix = "global::" + CombinedNamespace(component.Namespace, method.FullContextPrefix.Replace(".", string.Empty));
                    return $"        {contextPrefix}{component.ComponentPrefix}ComponentIndex.Index = new ComponentIndex({i});";
                }));
            }

            static string ComponentNames(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(component => $"            \"{component.FullName.RemoveSuffix("Component")}\""));
            }

            static string ComponentTypes(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(component => $"            typeof(global::{component.FullName})"));
            }
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }
    }
}
