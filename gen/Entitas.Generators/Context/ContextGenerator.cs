using System.Linq;
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
            var contextProvider = initContext.SyntaxProvider.CreateSyntaxProvider(
                    static (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                                        && !candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                                        && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                                        && !candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                                        && candidate.Modifiers.Any(SyntaxKind.PartialKeyword),
                    static ContextDeclaration? (syntaxContext, cancellationToken) =>
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
                    })
                .Where(context => context is not null)
                .Select((context, _) => context!.Value);

            initContext.RegisterSourceOutput(contextProvider, Execute);
        }

        static void Execute(SourceProductionContext spc, ContextDeclaration context)
        {
            ComponentIndex(spc, context);
            ContextAttribute(spc, context);
            ContextInitializationAttribute(spc, context);
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
                    public readonly struct ComponentIndex : System.IEquatable<ComponentIndex>
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

        static void ContextAttribute(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "ContextAttribute"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextAttribute))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    [System.Diagnostics.Conditional("false")]
                    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
                    public sealed class ContextAttribute : System.Attribute { }

                    """));
        }

        static void ContextInitializationAttribute(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "ContextInitializationAttribute"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationAttribute))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    [System.Diagnostics.Conditional("false")]
                    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
                    public sealed class ContextInitializationAttribute : System.Attribute { }

                    """));
        }

        static void Entity(SourceProductionContext spc, ContextDeclaration context)
        {
            spc.AddSource(ContextAwarePath(context, "Entity"),
                GeneratedFileHeader(GeneratorSource(nameof(Entity))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public sealed class Entity : Entitas.Entity { }

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
                        public static Entitas.IAllOfMatcher<Entity> AllOf(System.Span<ComponentIndex> indices)
                        {
                            return Entitas.Matcher<Entity>.AllOf(ToIntArray(indices));
                        }

                        public static Entitas.IAnyOfMatcher<Entity> AnyOf(System.Span<ComponentIndex> indices)
                        {
                            return Entitas.Matcher<Entity>.AnyOf(ToIntArray(indices));
                        }

                        public static Entitas.IAnyOfMatcher<Entity> AnyOf(this Entitas.IAllOfMatcher<Entity> matcher, System.Span<ComponentIndex> indices)
                        {
                            return matcher.AnyOf(ToIntArray(indices));
                        }

                        public static Entitas.INoneOfMatcher<Entity> NoneOf(this Entitas.IAnyOfMatcher<Entity> matcher, System.Span<ComponentIndex> indices)
                        {
                            return matcher.NoneOf(ToIntArray(indices));
                        }

                        static int[] ToIntArray(System.Span<ComponentIndex> indices)
                        {
                            var ints = new int[indices.Length];
                            for (var i = 0; i < indices.Length; i++)
                                ints[i] = indices[i].Value;

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
                    public sealed partial class {{context.Name}} : Entitas.Context<{{context.ContextPrefix}}.Entity>
                    {
                        public static string[] ComponentNames;
                        public static System.Type[] ComponentTypes;

                        public {{context.Name}}()
                            : base(
                                ComponentTypes.Length,
                                0,
                                new Entitas.ContextInfo(
                                    "{{context.FullName}}",
                                    ComponentNames,
                                    ComponentTypes
                                ),
                                entity =>
                    #if (ENTITAS_FAST_AND_UNSAFE)
                                    new Entitas.UnsafeAERC(),
                    #else
                                    new Entitas.SafeAERC(entity),
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
