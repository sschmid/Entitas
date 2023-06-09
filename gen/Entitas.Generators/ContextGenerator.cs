using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Generators;

[Generator(LanguageNames.CSharp)]
public class ContextGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var contexts = initContext.SyntaxProvider
            .CreateSyntaxProvider(SyntacticContextPredicate, SemanticContextTransform)
            .Where(context => context is not null);

        initContext.RegisterSourceOutput(contexts, (spc, context) => Execute(spc, context!.Value));
    }

    static bool SyntacticContextPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList: not null } candidate
               && !candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
               && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
               && !candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
               && candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
    }

    static ContextDeclaration? SemanticContextTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var candidate = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
        if (symbol is null)
            return null;

        var interfaceType = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(IContext).FullName);
        var isContext = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
        if (!isContext)
            return null;

        return new ContextDeclaration(
            !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null,
            symbol.ToDisplayString(),
            symbol.ToDisplayString(GeneratorUtils.NameOnlyFormat),
            symbol.Locations[0]
        );
    }

    static void Execute(SourceProductionContext spc, ContextDeclaration context)
    {
        GenerateEntity(spc, context);
        GenerateContext(spc, context);
    }

    static void GenerateEntity(SourceProductionContext spc, ContextDeclaration context)
    {
        spc.AddSource($"{Templates.CombineWithNamespace(context.EntityName, context.Namespace)}.g.cs",
            $$"""
            {{Templates.GeneratedFileHeader(typeof(ContextGenerator))}}
            {{Templates.NamespaceDeclaration(context.Namespace)}}
            public sealed partial class {{context.EntityName}} : Entitas.Entity { }

            """);
    }

    static void GenerateContext(SourceProductionContext spc, ContextDeclaration context)
    {
        spc.AddSource($"{context.Type}.g.cs",
            $$"""
            {{Templates.GeneratedFileHeader(typeof(ContextGenerator))}}
            {{Templates.NamespaceDeclaration(context.Namespace)}}
            public sealed partial class {{context.Name}} : Entitas.Context<{{context.EntityName}}>
            {
                public {{context.Name}}()
                    : base(
                        0,
                        0,
                        new Entitas.ContextInfo(
                            "{{context.ShortName}}",
                            null,
                            null
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

            """);
    }
}
