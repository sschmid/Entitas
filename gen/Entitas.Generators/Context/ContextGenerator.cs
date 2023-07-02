using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public sealed class ContextGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var contextChanged = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextCandidate, CreateContextDeclaration)
                .Where(context => context is not null)
                .Select((context, _) => context!.Value);

            initContext.RegisterSourceOutput(contextChanged, OnContextChanged);
        }

        static bool IsContextCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.BaseList.Types.Any(baseType => baseType.Type switch
                   {
                       IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier is { Text: "IContext" },
                       QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax is
                       {
                           Left: IdentifierNameSyntax { Identifier.Text: "Entitas" },
                           Right: IdentifierNameSyntax { Identifier.Text: "IContext" }
                       },
                       _ => false
                   })
                   && !candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static ContextDeclaration? CreateContextDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            var interfaceType = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IContext");
            if (interfaceType is null)
                return null;

            var isContext = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
            if (!isContext)
                return null;

            return new ContextDeclaration(symbol);
        }

        static void OnContextChanged(SourceProductionContext spc, ContextDeclaration context)
        {
            ComponentIndex(spc, context);
            Entity(spc, context);
            Matcher(spc, context);
            Context(spc, context);
        }

        static void ComponentIndex(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "ComponentIndex"),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public readonly struct ComponentIndex : global::System.IEquatable<ComponentIndex>
                    {
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

        static void Entity(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "Entity"),
                GeneratedFileHeader(GeneratorSource(nameof(Entity))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public sealed class Entity : global::Entitas.Entity { }

                    """));
        }

        static void Matcher(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "Matcher"),
                GeneratedFileHeader(GeneratorSource(nameof(Matcher))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public static class Matcher
                    {
                        public static global::Entitas.IAllOfMatcher<Entity> AllOf(params ComponentIndex[] indices)
                        {
                    #if NON_UNITY
                            var indexes = global::System.Runtime.CompilerServices.Unsafe.As<ComponentIndex[], int[]>(ref indices);
                    #else
                            var indexes = global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<ComponentIndex[], int[]>(ref indices);
                    #endif
                            return global::Entitas.Matcher<Entity>.AllOf(indexes);
                        }

                        public static global::Entitas.IAnyOfMatcher<Entity> AnyOf(params ComponentIndex[] indices)
                        {
                    #if NON_UNITY
                            var indexes = global::System.Runtime.CompilerServices.Unsafe.As<ComponentIndex[], int[]>(ref indices);
                    #else
                            var indexes = global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<ComponentIndex[], int[]>(ref indices);
                    #endif
                            return global::Entitas.Matcher<Entity>.AnyOf(indexes);
                        }

                        public static global::Entitas.IAnyOfMatcher<Entity> AnyOf(this global::Entitas.IAllOfMatcher<Entity> matcher, params ComponentIndex[] indices)
                        {
                    #if NON_UNITY
                            var indexes = global::System.Runtime.CompilerServices.Unsafe.As<ComponentIndex[], int[]>(ref indices);
                    #else
                            var indexes = global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<ComponentIndex[], int[]>(ref indices);
                    #endif
                            return matcher.AnyOf(indexes);
                        }

                        public static global::Entitas.INoneOfMatcher<Entity> NoneOf(this global::Entitas.IAnyOfMatcher<Entity> matcher, params ComponentIndex[] indices)
                        {
                            var indexes =
                    #if NON_UNITY
                            var indexes = global::System.Runtime.CompilerServices.Unsafe.As<ComponentIndex[], int[]>(ref indices);
                    #else
                            var indexes = global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<ComponentIndex[], int[]>(ref indices);
                    #endif
                            return matcher.NoneOf(indexes);
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
                    public sealed partial class {{context.Name}} : global::Entitas.Context<{{context.ContextPrefix}}.Entity>
                    {
                        public static string[] ComponentNames;
                        public static global::System.Type[] ComponentTypes;

                        public {{context.Name}}()
                            : base(
                                ComponentTypes.Length,
                                0,
                                new global::Entitas.ContextInfo(
                                    "{{context.FullName}}",
                                    ComponentNames,
                                    ComponentTypes
                                ),
                                entity =>
                    #if (ENTITAS_FAST_AND_UNSAFE)
                                    new global::Entitas.UnsafeAERC(),
                    #else
                                    new global::Entitas.SafeAERC(entity),
                    #endif
                                () => new {{context.ContextPrefix}}.Entity()
                            ) { }
                    }

                    """));
        }

        static string ContextAwarePath(ContextDeclaration context, string hintName)
        {
            return GeneratedPath($"{context.FullContextPrefix}.{hintName}");
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ContextGenerator).FullName}.{source}";
        }
    }
}
