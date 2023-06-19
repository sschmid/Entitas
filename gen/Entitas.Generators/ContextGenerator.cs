using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public class ContextGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var contextDeclarationOrDiagnostic = initContext.SyntaxProvider
                .CreateSyntaxProvider(SyntacticContextPredicate, SemanticContextTransform)
                .Where(context => context is not null);

            var diagnostics = contextDeclarationOrDiagnostic
                .Where(x => x.Diagnostic is not null)
                .Select((s, _) => s.Diagnostic!);

            var contextDeclarations = contextDeclarationOrDiagnostic
                .Where(x => x.Result is not null)
                .Select((s, _) => s.Result!.Value);

            initContext.RegisterSourceOutput(diagnostics, EntitasDiagnostics.ReportDiagnostics);
            initContext.RegisterSourceOutput(contextDeclarations, Execute);
        }

        static bool SyntacticContextPredicate(SyntaxNode node, CancellationToken cancellationToken)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && !candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static INamedTypeSymbol? _contextInterfaceTypeSymbol;
        static ResultOrDiagnostics<ContextDeclaration?> SemanticContextTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return Diagnostic.Create(EntitasDiagnostics.NamedTypeSymbolNotFound, Location.None, candidate.Identifier.Text);

            const string interfaceName = "Entitas.IContext";
            _contextInterfaceTypeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName(interfaceName);
            if (_contextInterfaceTypeSymbol is null)
                return Diagnostic.Create(EntitasDiagnostics.CouldNotFindInterface, Location.None, interfaceName);

            var isContext = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(_contextInterfaceTypeSymbol, SymbolEqualityComparer.Default));
            if (!isContext)
                return new ResultOrDiagnostics<ContextDeclaration?>(result: null);

            return new ContextDeclaration(symbol);
        }

        static void Execute(SourceProductionContext spc, ContextDeclaration context)
        {
            ComponentIndex(spc, context);
            ContextAttribute(spc, context);
            Entity(spc, context);
            Matcher(spc, context);
            Context(spc, context);
        }

        static void ComponentIndex(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "ComponentIndex"),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                NamespaceDeclaration(context.Namespace, context.ContextPrefix,
                    """
                    public readonly struct ComponentIndex : System.IEquatable<ComponentIndex>
                    {
                        public static implicit operator int(ComponentIndex index) => index.Value;
                        public static implicit operator ComponentIndex(int index) => new ComponentIndex(index);

                        public readonly int Value;

                        public ComponentIndex(int value)
                        {
                            Value = value;
                        }

                        public bool Equals(ComponentIndex other) => Value == other.Value;
                    #nullable enable
                        public override bool Equals(object? obj) => obj is ComponentIndex other && Equals(other);
                    #nullable disable
                        public override int GetHashCode() => Value;
                    }

                    """));
        }

        static void ContextAttribute(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "ContextAttribute"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextAttribute))) +
                NamespaceDeclaration(context.Namespace, context.ContextPrefix,
                    """
                    public sealed class ContextAttribute : Entitas.Generators.Attributes.ContextAttribute { }

                    """));
        }

        static void Entity(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "Entity"),
                GeneratedFileHeader(GeneratorSource(nameof(Entity))) +
                NamespaceDeclaration(context.Namespace, context.ContextPrefix,
                    $$"""
                    public sealed class Entity : Entitas.Entity { }

                    """));
        }

        static void Matcher(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "Matcher"),
                GeneratedFileHeader(GeneratorSource(nameof(Matcher))) +
                NamespaceDeclaration(context.Namespace, context.ContextPrefix,
                    $$"""
                    public static class Matcher
                    {
                        public static Entitas.IAllOfMatcher<{{context.EntityName}}> AllOf(System.Span<ComponentIndex> indices)
                        {
                            return Entitas.Matcher<{{context.EntityName}}>.AllOf(ToIntArray(indices));
                        }

                        public static Entitas.IAnyOfMatcher<{{context.EntityName}}> AnyOf(System.Span<ComponentIndex> indices)
                        {
                            return Entitas.Matcher<{{context.EntityName}}>.AnyOf(ToIntArray(indices));
                        }

                        public static Entitas.IAnyOfMatcher<{{context.EntityName}}> AnyOf(this Entitas.IAllOfMatcher<{{context.EntityName}}> matcher, System.Span<ComponentIndex> indices)
                        {
                            return matcher.AnyOf(ToIntArray(indices));
                        }

                        public static Entitas.INoneOfMatcher<{{context.EntityName}}> NoneOf(this Entitas.IAnyOfMatcher<{{context.EntityName}}> matcher, System.Span<ComponentIndex> indices)
                        {
                            return matcher.NoneOf(ToIntArray(indices));
                        }

                        static int[] ToIntArray(System.Span<ComponentIndex> indices)
                        {
                            var ints = new int[indices.Length];
                            for (var i = 0; i < indices.Length; i++)
                                ints[i] = indices[i];

                            return ints;
                        }
                    }

                    """));
        }

        static void Context(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(GeneratedPath(context.FullName),
                GeneratedFileHeader(GeneratorSource(nameof(Context))) +
                NamespaceDeclaration(context.Namespace,
                    $$"""
                    public sealed partial class {{context.Name}} : Entitas.Context<{{context.EntityName}}>
                    {
                        public {{context.Name}}()
                            : base(
                                0,
                                0,
                                new Entitas.ContextInfo(
                                    "{{context.FullName}}",
                                    System.Array.Empty<string>(),
                                    System.Array.Empty<System.Type>()
                                ),
                                entity =>
                    #if (ENTITAS_FAST_AND_UNSAFE)
                                    new Entitas.UnsafeAERC(),
                    #else
                                    new Entitas.SafeAERC(entity),
                    #endif
                                () => new {{context.EntityName}}()
                            ) { }
                    }

                    """));
        }

        static string ContextAwarePath(ContextDeclaration context, string hintName)
        {
            return GeneratedPath($"{CombinedNamespace(context.Namespace, context.ContextPrefix)}.{hintName}");
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ContextGenerator).FullName}.{source}";
        }

        public readonly struct ContextDeclaration : IEquatable<ContextDeclaration>
        {
            /// When: MyApp.MainContext
            /// Then: MyApp
            public readonly string? Namespace;

            /// When: MyApp.MainContext
            /// Then: MyApp.MainContext
            public readonly string FullName;

            /// When: MyApp.MainContext
            /// Then: MainContext
            public readonly string Name;

            public readonly Location Location;

            /// When: MyApp.MainContext
            /// Then: Main
            public readonly string ContextPrefix;

            /// When: MyApp.MainContext
            /// Then: Main.Entity
            public readonly string EntityName;

            public ContextDeclaration(INamedTypeSymbol symbol)
            {
                Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
                FullName = symbol.ToDisplayString();
                Name = symbol.Name;

                Location = symbol.Locations.FirstOrDefault() ?? Location.None;

                ContextPrefix = Name.RemoveSuffix("Context");
                EntityName = $"{ContextPrefix}.Entity";
            }

            public bool Equals(ContextDeclaration other) =>
                Namespace == other.Namespace &&
                FullName == other.FullName &&
                Name == other.Name;

            public override bool Equals(object? obj) => obj is ContextDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Namespace, FullName, Name);
        }
    }
}
