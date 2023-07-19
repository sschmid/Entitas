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
            var optionsProvider = initContext.AnalyzerConfigOptionsProvider;
            var componentsProvider = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclaration)
                .Where(static component => component is not null)
                .Select(static (component, _) => component!.Value);

            initContext.RegisterSourceOutput(
                componentsProvider.WithComparer(FullNameAndContextsComparer.Instance).Combine(optionsProvider),
                static (SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                {
                    foreach (var context in pair.Component.Contexts)
                    {
                        ComponentIndex(spc, pair.Component, context, pair.OptionsProvider);
                        Matcher(spc, pair.Component, context, pair.OptionsProvider);
                    }
                });

            initContext.RegisterSourceOutput(
                componentsProvider.WithComparer(new FullNameAndMembersAndContextsComparer(TypeAndNameComparer.Instance)).Combine(optionsProvider),
                static (SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                {
                    foreach (var context in pair.Component.Contexts)
                    {
                        EntityExtension(spc, pair.Component, context, pair.OptionsProvider);
                    }
                });

            initContext.RegisterSourceOutput(
                componentsProvider.WithComparer(new FullNameAndMembersAndContextsAndIsUniqueComparer(TypeAndNameComparer.Instance)).Combine(optionsProvider),
                static (SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                {
                    foreach (var context in pair.Component.Contexts)
                    {
                        ContextExtension(spc, pair.Component, context, pair.OptionsProvider);
                    }
                });

            initContext.RegisterSourceOutput(
                componentsProvider.WithComparer(FullNameAndContextsAndCleanupModeComparer.Instance).Combine(optionsProvider),
                static (SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                {
                    foreach (var context in pair.Component.Contexts)
                    {
                        CleanupSystem(spc, pair.Component, context, pair.OptionsProvider);
                    }
                });

            initContext.RegisterSourceOutput(
                componentsProvider.WithComparer(new FullNameAndMembersAndContextsAndEventsComparer(TypeAndNameComparer.Instance, EventTargetAndEventTypeComparer.Instance)).Combine(optionsProvider),
                static (SourceProductionContext spc, (ComponentDeclaration Component, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                {
                    foreach (var context in pair.Component.Contexts)
                    {
                        Events(spc, pair.Component, context, pair.OptionsProvider);
                    }
                });

            var componentsInCompilationProvider = initContext.CompilationProvider.Select(CreateComponentDeclarationsForCompilation);

            var contextInitializationProvider = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(static method => method is not null)
                .Select(static (method, _) => method!.Value)
                .WithComparer(NamespaceAndClassAndNameAndContextFullNameComparer.Instance);

            var contextInitializationMethodComparer = new FullNameAndContextsAndEventsComparer(EventTargetAndEventTypeComparer.Instance);
            initContext.RegisterImplementationSourceOutput(contextInitializationProvider
                    .Combine(componentsInCompilationProvider.WithComparer(new ComponentsComparer(contextInitializationMethodComparer)))
                    .Select(static ((ContextInitializationMethodDeclaration Method, ImmutableArray<ComponentDeclaration> Components) pair, CancellationToken _) =>
                    {
                        pair.Method.Components = pair.Components
                            .Where(component => component.Contexts.Contains(pair.Method.ContextFullName))
                            .ToImmutableArray();

                        return pair.Method;
                    })
                    .WithComparer(new NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer(contextInitializationMethodComparer))
                    .Combine(optionsProvider),
                static (SourceProductionContext spc, (ContextInitializationMethodDeclaration Method, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                    ContextInitializationMethod(spc, pair.Method, pair.OptionsProvider));

            var cleanupSystemsComparer = FullNameAndContextsAndCleanupModeComparer.Instance;
            initContext.RegisterSourceOutput(contextInitializationProvider
                    .Combine(componentsInCompilationProvider.WithComparer(new ComponentsComparer(cleanupSystemsComparer)))
                    .Select(static ((ContextInitializationMethodDeclaration Method, ImmutableArray<ComponentDeclaration> Components) pair, CancellationToken _) =>
                    {
                        pair.Method.Components = pair.Components
                            .Where(static component => component.CleanupMode != -1)
                            .Where(component => component.Contexts.Contains(pair.Method.ContextFullName))
                            .ToImmutableArray();

                        return pair.Method;
                    })
                    .WithComparer(new NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer(cleanupSystemsComparer))
                    .Combine(optionsProvider),
                static (SourceProductionContext spc, (ContextInitializationMethodDeclaration Method, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                    CleanupSystems(spc, pair.Method, pair.OptionsProvider));

            var eventSystemsContextExtensionComparer = new FullNameAndContextsAndEventsComparer(EventTargetAndEventTypeAndOrderComparer.Instance);
            initContext.RegisterSourceOutput(contextInitializationProvider
                    .Combine(componentsInCompilationProvider.WithComparer(new ComponentsComparer(eventSystemsContextExtensionComparer)))
                    .Select(static ((ContextInitializationMethodDeclaration Method, ImmutableArray<ComponentDeclaration> Components) pair, CancellationToken _) =>
                    {
                        pair.Method.Components = pair.Components
                            .Where(component => component.Events.Length > 0)
                            .Where(component => component.Contexts.Contains(pair.Method.ContextFullName))
                            .ToImmutableArray();

                        return pair.Method;
                    })
                    .WithComparer(new NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer(eventSystemsContextExtensionComparer))
                    .Combine(optionsProvider),
                static (SourceProductionContext spc, (ContextInitializationMethodDeclaration Method, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                    EventSystemsContextExtension(spc, pair.Method, pair.OptionsProvider));

            var entityIndexExtensionComparer = new FullNameAndMembersAndContextsComparer(TypeAndNameAndEntityIndexTypeComparer.Instance);
            initContext.RegisterSourceOutput(contextInitializationProvider
                    .Combine(componentsInCompilationProvider.WithComparer(new ComponentsComparer(entityIndexExtensionComparer)))
                    .Select(static ((ContextInitializationMethodDeclaration Method, ImmutableArray<ComponentDeclaration> Components) pair, CancellationToken _) =>
                    {
                        pair.Method.Components = pair.Components
                            .Where(static component => component.Members.Any(static member => member.EntityIndexType != -1))
                            .Where(component => component.Contexts.Contains(pair.Method.ContextFullName))
                            .ToImmutableArray();

                        return pair.Method;
                    })
                    .WithComparer(new NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer(entityIndexExtensionComparer))
                    .Combine(optionsProvider),
                static (SourceProductionContext spc, (ContextInitializationMethodDeclaration Method, AnalyzerConfigOptionsProvider OptionsProvider) pair) =>
                    EntityIndexExtension(spc, pair.Method, pair.OptionsProvider));
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

            var contexts = GetContexts(symbol);
            if (contexts.Length == 0)
                return null;

            return new ComponentDeclaration(candidate.SyntaxTree, symbol, contexts, cancellationToken);
        }

        static ImmutableArray<ComponentDeclaration> CreateComponentDeclarationsForCompilation(Compilation compilation, CancellationToken cancellationToken)
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

                        var contexts = GetContexts(symbol);
                        if (contexts.Length == 0)
                            continue;

                        var component = new ComponentDeclaration(symbol.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree, symbol, contexts, cancellationToken);
                        allComponents.Add(component);

                        foreach (var context in contexts)
                        {
                            var contextAware = ContextAware(ContextPrefix(context));
                            foreach (var @event in component.Events)
                            {
                                @event.ContextAware(contextAware);
                                allComponents.Add(ToEvent(component, @event));
                            }
                        }
                    }
                }
            }

            return allComponents
                .OrderBy(static component => component.FullName)
                .ToImmutableArray();
        }

        static ImmutableArray<string> GetContexts(INamedTypeSymbol symbol)
        {
            return symbol.GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextAttribute")
                .Select(static attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(static arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(static arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct()
                .ToImmutableArray();
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

            return new ContextInitializationMethodDeclaration(candidate.SyntaxTree, symbol, context);
        }

        static ComponentDeclaration ToEvent(ComponentDeclaration component, EventDeclaration @event) => component.ToEvent(
            CombinedNamespace(component.Namespace, @event.ContextAwareEventListenerComponent),
            @event.ContextAwareEventListenerComponent,
            ImmutableArray.Create(new MemberDeclaration($"global::System.Collections.Generic.List<{@event.ContextAwareEventListenerInterface}>", "Value", -1)),
            @event.EventListener);

        static string GeneratorSource(string source) => $"{typeof(ComponentGenerator).FullName}.{source}";
    }
}
