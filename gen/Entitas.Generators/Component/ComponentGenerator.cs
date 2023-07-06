using System.Collections.Generic;
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
            var components = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclarations)
                .Where(static components => components is not null)
                .SelectMany(static (components, _) => components!.Value);

            var fullNameOrContextChanged = components.WithComparer(new FullNameAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrContextChanged, OnFullNameOrContextChanged);

            var fullNameOrMembersOrContextChanged = components.WithComparer(new FullNameAndMembersAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextChanged, OnFullNameOrMembersOrContextChanged);

            var fullNameOrMembersOrContextOrIsUniqueChanged = components.WithComparer(new FullNameAndMembersAndContextAndIsUniqueComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextOrIsUniqueChanged, OnFullNameOrMembersOrContextOrIsUniqueChanged);

            var contextInitializationChanged = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(static method => method is not null)
                .Select(static (method, _) => method!.Value);

            initContext.RegisterImplementationSourceOutput(contextInitializationChanged, OnContextInitializationChanged);
        }

        static bool IsComponentCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.BaseList.Types.Any(static baseType => baseType.Type switch
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

        static ImmutableArray<ComponentDeclaration>? CreateComponentDeclarations(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            var componentInterface = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (componentInterface is null)
                return null;

            var isComponent = symbol.Interfaces.Contains(componentInterface);
            if (!isComponent)
                return null;

            return GetContexts(symbol)
                .Select(context => new ComponentDeclaration(symbol, context, cancellationToken))
                .ToImmutableArray();
        }

        static IEnumerable<string> GetContexts(INamedTypeSymbol symbol)
        {
            return symbol.GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextAttribute")
                .Select(static attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(static arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(static arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct();
        }

        static bool IsContextInitializationMethodCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                   && candidate.AttributeLists.Any(static attributeList => attributeList.Attributes
                       .Any(static attribute => attribute.Name is IdentifierNameSyntax { Identifier.Text: "ContextInitialization" or "ContextInitializationAttribute" }))
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
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextInitializationAttribute")
                .Select(static attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(static arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(static arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct()
                .SingleOrDefault();

            if (context is null)
                return null;

            var components = GetOrderedComponentsFromAllAssemblies(context, syntaxContext.SemanticModel.Compilation, cancellationToken);
            if (components is null)
                return null;

            return new ContextInitializationMethodDeclaration(symbol, context, components.Value);
        }

        static ImmutableArray<ComponentDeclaration>? GetOrderedComponentsFromAllAssemblies(string context, Compilation compilation, CancellationToken cancellationToken)
        {
            var componentInterface = compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (componentInterface is null)
                return null;

            var allComponents = new List<ComponentDeclaration>();
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(compilation.GlobalNamespace);

            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var member in stack.Pop().GetMembers())
                {
                    if (member is INamespaceSymbol ns)
                    {
                        stack.Push(ns);
                    }
                    else if (member is INamedTypeSymbol symbol)
                    {
                        var isComponent = symbol.Interfaces.Contains(componentInterface);
                        if (!isComponent)
                            continue;

                        if (!GetContexts(symbol).Contains(context))
                            continue;

                        allComponents.Add(new ComponentDeclaration(symbol, context, cancellationToken));
                    }
                }
            }

            return allComponents
                .OrderBy(static component => component.FullName)
                .ToImmutableArray();
        }

        static void OnFullNameOrContextChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            ComponentIndex(spc, component);
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component)
        {
            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.ContextPrefix}.ComponentIndex"),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using global::{component.ContextPrefix};\n\n" +
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
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                        {
                            var index = Index.Value;
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
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
                GeneratedPath($"{component.FullName}.{component.ContextPrefix}.EntityExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                $"using global::{component.ContextPrefix};\n" +
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
                        public static bool Has{{component.ComponentPrefix}}(this {{component.Context}} context, {{ComponentMethodArgs(component)}})
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{component.Context}} context, {{ComponentMethodArgs(component)}})
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            if (entity == null)
                                entity = context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodParams(component)}});
                            else
                                entity.Replace{{component.ComponentPrefix}}({{ComponentMethodParams(component)}});

                            return entity;
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{component.Context}} context)
                        {
                            return context.GetGroup(Matcher.AllOf(Index)).GetSingleEntity();
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            return entity != null ? entity.Get{{component.ComponentPrefix}}() : null;
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() ?? context.CreateEntity().Add{{component.ComponentPrefix}}();
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{component.Context}} context)
                        {
                            return context.GetGroup(Matcher.AllOf(Index)).GetSingleEntity();
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this {{component.Context}} context)
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            return entity != null ? entity.Get{{component.ComponentPrefix}}() : null;
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.ContextPrefix}.ContextExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextExtension))) +
                DisableNullable() +
                $"using global::{component.ContextPrefix};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, component.ContextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static string ComponentMethodArgs(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.Type} {member.ValidLowerFirstName}"));
        }

        static string ComponentMethodParams(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.ValidLowerFirstName}"));
        }

        static string ComponentValueAssignments(ComponentDeclaration component)
        {
            return string.Join("\n", component.Members.Select(static member =>
                $$"""
                        component.{{member.Name}} = {{member.ValidLowerFirstName}};
                """));
        }

        static void OnContextInitializationChanged(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
        {
            ContextInitializationMethod(spc, method);
        }

        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
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
                    {{ComponentIndexAssignments(method, method.Components)}}

                            {{method.Context}}.ComponentNames = new string[]
                            {
                    {{ComponentNames(method.Components)}}
                            };

                            {{method.Context}}.ComponentTypes = new global::System.Type[]
                            {
                    {{ComponentTypes(method.Components)}}
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
                return string.Join(",\n", components.Select(static component => $"            \"{component.FullName.RemoveSuffix("Component")}\""));
            }

            static string ComponentTypes(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(static component => $"            typeof(global::{component.FullName})"));
            }
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }
    }
}
