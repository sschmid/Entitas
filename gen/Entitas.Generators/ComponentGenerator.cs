using System;
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
    public class ComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var componentDeclarationOrDiagnostic = initContext.SyntaxProvider
                .CreateSyntaxProvider(SyntacticComponentPredicate, SemanticComponentTransform);

            var diagnostics = componentDeclarationOrDiagnostic
                .Where(x => x.Diagnostic is not null)
                .Select((s, _) => s.Diagnostic!);

            var componentDeclarations = componentDeclarationOrDiagnostic
                .Where(x => x.Result is not null)
                .Select((s, _) => s.Result!.Value);

            initContext.RegisterSourceOutput(diagnostics, EntitasDiagnostics.ReportDiagnostics);
            initContext.RegisterSourceOutput(componentDeclarations, Execute);
        }

        static bool SyntacticComponentPredicate(SyntaxNode node, CancellationToken cancellationToken)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && !candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static INamedTypeSymbol? _componentInterfaceTypeSymbol;

        static ResultOrDiagnostics<ComponentDeclaration?> SemanticComponentTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return Diagnostic.Create(EntitasDiagnostics.NamedTypeSymbolNotFound, Location.None, candidate.Identifier.Text);

            const string interfaceName = "Entitas.IComponent";
            _componentInterfaceTypeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName(interfaceName);
            if (_componentInterfaceTypeSymbol is null)
                return Diagnostic.Create(EntitasDiagnostics.CouldNotFindInterface, Location.None, interfaceName);

            var isComponent = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(_componentInterfaceTypeSymbol, SymbolEqualityComparer.Default));
            if (!isComponent)
                return new ResultOrDiagnostics<ComponentDeclaration?>(result: null);

            return new ComponentDeclaration(symbol, context, cancellationToken);
        }

        static void Execute(SourceProductionContext spc, ComponentDeclaration component)
        {
            ComponentIndex(spc, component);
            EntityExtensions(spc, component);
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                var className = $"{component.FullComponentPrefix}ComponentIndex";
                spc.AddSource(
                    GeneratedPath($"{context}.{className}"),
                    GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                    NamespaceDeclaration(context,
                        $$"""
                        public static class {{className}}
                        {
                            public static ComponentIndex Value;
                        }

                        """));
            }
        }

        static void EntityExtensions(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                var className = $"{component.FullComponentPrefix}EntityExtensions";
                var index = $"{component.FullComponentPrefix}ComponentIndex.Value";
                spc.AddSource(GeneratedPath($"{context}.{className}"),
                    GeneratedFileHeader(GeneratorSource(nameof(EntityExtensions))) +
                    NamespaceDeclaration(context,
                        $$"""
                        public static class {{className}}
                        {
                            static readonly {{component.FullName}} Single{{component.Name}} = new {{component.FullName}}();

                            public static bool Has{{component.ComponentPrefix}}(this {{context}}.Entity entity)
                            {
                                return entity.HasComponent({{index}});
                            }

                            public static {{context}}.Entity Add{{component.ComponentPrefix}}(this {{context}}.Entity entity)
                            {
                                entity.AddComponent({{index}}, Single{{component.Name}});
                                return entity;
                            }

                            public static {{context}}.Entity Replace{{component.ComponentPrefix}}(this {{context}}.Entity entity)
                            {
                                entity.ReplaceComponent({{index}}, Single{{component.Name}});
                                return entity;
                            }

                            public static {{context}}.Entity Remove{{component.ComponentPrefix}}(this {{context}}.Entity entity)
                            {
                                entity.RemoveComponent({{index}});
                                return entity;
                            }
                        }

                        """));
            }
        }

        static string MemberExtensions(ImmutableArray<MemberDeclaration> members)
        {
            return string.Join("\n\n", members.Select(member =>
                $$"""
                    public static {{member.Type}} Get{{member.Name}}(this object entity)
                    {
                        return default;
                    }
                """));
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }

        public readonly struct ComponentDeclaration : IEquatable<ComponentDeclaration>
        {
            /// When: MyApp.SomeComponent
            /// Then: MyApp
            public readonly string? Namespace;

            /// When: MyApp.SomeComponent
            /// Then: MyApp.SomeComponent
            public readonly string FullName;

            /// When: MyApp.SomeComponent
            /// Then: SomeComponent
            public readonly string Name;

            public readonly ImmutableArray<MemberDeclaration> Members;
            public readonly ImmutableArray<string> Contexts;

            public readonly Location Location;

            /// When: MyApp.SomeComponent
            /// Then: MyAppSome
            public readonly string FullComponentPrefix;

            /// When: MyApp.SomeComponent
            /// Then: Some
            public readonly string ComponentPrefix;

            public ComponentDeclaration(INamedTypeSymbol symbol, GeneratorSyntaxContext context, CancellationToken cancellationToken)
            {
                Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
                FullName = symbol.ToDisplayString();
                Name = symbol.Name;

                Members = symbol.GetMembers()
                    .Where(member => member is IFieldSymbol or IPropertySymbol
                    {
                        DeclaredAccessibility: Accessibility.Public,
                        IsStatic: false,
                    })
                    .Select(member =>
                    {
                        var memberType = member switch
                        {
                            IFieldSymbol field => field.Type,
                            IPropertySymbol property => property.Type,
                            _ => throw new ArgumentOutOfRangeException(nameof(member), member, null)
                        };

                        return new MemberDeclaration(memberType.ToDisplayString(), member.Name);
                    })
                    .ToImmutableArray();

                Contexts = symbol.GetAttributes()
                    .Select(attribute => attribute.AttributeClass?.ToDisplayString())
                    .Where(attribute => attribute?.EndsWith(".Context") ?? false)
                    .Select(attribute => attribute!.RemoveSuffix(".Context"))
                    .ToImmutableArray();

                Location = symbol.Locations.FirstOrDefault() ?? Location.None;

                FullComponentPrefix = FullName.Replace(".", string.Empty).RemoveSuffix("Component");
                ComponentPrefix = Name.RemoveSuffix("Component");
            }

            public bool Equals(ComponentDeclaration other) =>
                Namespace == other.Namespace &&
                FullName == other.FullName &&
                Name == other.Name &&
                Members.SequenceEqual(other.Members) &&
                Contexts.SequenceEqual(other.Contexts);

            public override bool Equals(object? obj) => obj is ComponentDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Namespace, FullName, Name, Members, Contexts);
        }

        public readonly struct MemberDeclaration : IEquatable<MemberDeclaration>
        {
            public readonly string Type;
            public readonly string Name;

            public MemberDeclaration(string type, string name)
            {
                Type = type;
                Name = name;
            }

            public bool Equals(MemberDeclaration other) => Type == other.Type && Name == other.Name;
            public override bool Equals(object? obj) => obj is MemberDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Type, Name);
        }
    }
}
