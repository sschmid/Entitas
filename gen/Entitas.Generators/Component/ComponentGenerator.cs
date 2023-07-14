using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public sealed partial class ComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var options = initContext.AnalyzerConfigOptionsProvider;

            var components = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclaration)
                .Where(static component => component is not null)
                .Select(static (component, _) => component!.Value);

            var fullNameOrContextsChanged = components.WithComparer(new FullNameAndContextsComparer());
            initContext.RegisterSourceOutput(fullNameOrContextsChanged.Combine(options), OnFullNameOrContextsChanged);

            var fullNameOrMembersOrContextsChanged = components.WithComparer(new FullNameAndMembersAndContextsComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextsChanged.Combine(options), OnFullNameOrMembersOrContextsChanged);

            var fullNameOrMembersOrContextsOrIsUniqueChanged = components.WithComparer(new FullNameAndMembersAndContextsAndIsUniqueComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextsOrIsUniqueChanged.Combine(options), OnFullNameOrMembersOrContextsOrIsUniqueChanged);

            var fullNameOrContextsOrEventsChanged = components.WithComparer(new FullNameAndContextsAndEventsComparer());
            initContext.RegisterSourceOutput(fullNameOrContextsOrEventsChanged.Combine(options), OnFullNameOrContextsOrEventsChanged);

            var contextInitializationChanged = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(static method => method is not null)
                .Select(static (method, _) => method!.Value);

            initContext.RegisterImplementationSourceOutput(contextInitializationChanged.Combine(options), OnContextInitializationChanged);
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

        static ComponentDeclaration? CreateComponentDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
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

            return new ComponentDeclaration(candidate, symbol, cancellationToken);
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
                .Select(static arg => (INamedTypeSymbol)arg.Value!)
                .Distinct(SymbolEqualityComparer.Default)
                .SingleOrDefault();

            if (context is null)
                return null;

            var components = GetOrderedComponentsFromAllAssemblies(context.ToDisplayString(), syntaxContext.SemanticModel.Compilation, cancellationToken);
            return new ContextInitializationMethodDeclaration(candidate, symbol, context, components);

            static ImmutableArray<ComponentDeclaration> GetOrderedComponentsFromAllAssemblies(string context, Compilation compilation, CancellationToken cancellationToken)
            {
                var componentInterface = compilation.GetTypeByMetadataName("Entitas.IComponent");
                if (componentInterface is null)
                    return ImmutableArray<ComponentDeclaration>.Empty;

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

                            if (!ComponentDeclaration.GetContexts(symbol).Contains(context))
                                continue;

                            var component = new ComponentDeclaration(null, symbol, cancellationToken);
                            allComponents.Add(component);

                            var contextPrefix = component.ContextPrefix(context);
                            var contextAware = component.ContextAware(contextPrefix);
                            foreach (var @event in component.Events)
                            {
                                var eventStrings = new EventStrings(@event, component.Prefix, contextAware);
                                allComponents.Add(ToEvent(component, eventStrings));
                            }
                        }
                    }
                }

                return allComponents
                    .OrderBy(static component => component.FullName)
                    .ToImmutableArray();
            }
        }

        static void OnFullNameOrContextsChanged(SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider optionsProvider) pair)
        {
            var (component, optionsProvider) = pair;
            foreach (var context in component.Contexts)
            {
                OnFullNameOrContextsChanged(spc, component, context, optionsProvider);
            }
        }

        static void OnFullNameOrContextsChanged(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            ComponentIndex(spc, component, context, optionsProvider);
            Matcher(spc, component, context, optionsProvider);
        }

        static void OnFullNameOrMembersOrContextsChanged(SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider optionsProvider) pair)
        {
            var (component, optionsProvider) = pair;
            foreach (var context in component.Contexts)
            {
                OnFullNameOrMembersOrContextsChanged(spc, component, context, optionsProvider);
            }
        }

        static void OnFullNameOrMembersOrContextsChanged(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            EntityExtension(spc, component, context, optionsProvider);
        }

        static void OnFullNameOrMembersOrContextsOrIsUniqueChanged(SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider optionsProvider) pair)
        {
            var (component, optionsProvider) = pair;
            foreach (var context in component.Contexts)
            {
                OnFullNameOrMembersOrContextsOrIsUniqueChanged(spc, component, context, optionsProvider);
            }
        }

        static void OnFullNameOrMembersOrContextsOrIsUniqueChanged(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            ContextExtension(spc, component, context, optionsProvider);
        }

        static void OnFullNameOrContextsOrEventsChanged(SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider optionsProvider) pair)
        {
            var (component, optionsProvider) = pair;
            foreach (var context in component.Contexts)
            {
                OnFullNameOrContextsOrEventsChanged(spc, component, context, optionsProvider);
            }
        }

        static void OnFullNameOrContextsOrEventsChanged(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            Events(spc, component, context, optionsProvider);
        }

        static void OnContextInitializationChanged(SourceProductionContext spc, (ContextInitializationMethodDeclaration Method, AnalyzerConfigOptionsProvider optionsProvider) pair)
        {
            var (method, optionsProvider) = pair;
            ContextInitializationMethod(spc, method, optionsProvider);
            EventSystemsContextExtension(spc, method, optionsProvider);
        }

        static ComponentDeclaration ToEvent(ComponentDeclaration component, EventStrings eventStrings)
        {
            return component.ToEvent(
                CombinedNamespace(component.Namespace, eventStrings.ContextAwareEventListenerComponent),
                eventStrings.ContextAwareEventListenerComponent,
                ImmutableArray.Create(new MemberDeclaration($"global::System.Collections.Generic.List<{eventStrings.ContextAwareEventListenerInterface}>", "Value")),
                eventStrings.EventListener);
        }

        static string ComponentMethodParams(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.Type} {member.ValidLowerFirstName}"));
        }

        static string ComponentMethodArgs(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.ValidLowerFirstName}"));
        }

        static string ComponentValueMethodArgs(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"component.{member.Name}"));
        }

        static string ComponentValueAssignments(ComponentDeclaration component)
        {
            return string.Join("\n", component.Members.Select(static member => $"        component.{member.Name} = {member.ValidLowerFirstName};"));
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }
    }
}
