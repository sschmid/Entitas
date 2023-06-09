using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Generators;

[Generator(LanguageNames.CSharp)]
public class ComponentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var components = initContext.SyntaxProvider
            .CreateSyntaxProvider(SyntacticComponentPredicate, SemanticComponentTransform)
            .Where(component => component is not null);

        initContext.RegisterSourceOutput(components, (context, component) => Execute(context, component!.Value));
    }

    static bool SyntacticComponentPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList: not null } candidate
               && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
               && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
               && candidate.Modifiers.Any(SyntaxKind.SealedKeyword);
    }

    static ComponentDeclaration? SemanticComponentTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var candidate = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
        if (symbol is null)
            return null;

        var type = GeneratorUtils.GetComponentInterfaceType(context);
        var isComponent = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(type, SymbolEqualityComparer.Default));
        if (!isComponent)
            return null;

        return new ComponentDeclaration(
            !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null,
            symbol.ToDisplayString(),
            symbol.ToDisplayString(GeneratorUtils.NameOnlyFormat),
            symbol.GetMembers()
                .Where(member => member.DeclaredAccessibility == Accessibility.Public)
                .Select<ISymbol, MemberDeclaration?>(member =>
                {
                    var memberType = member switch
                    {
                        IFieldSymbol field => field.Type,
                        IPropertySymbol property => property.Type,
                        _ => null
                    };

                    if (memberType is null)
                        return null;

                    return new MemberDeclaration(
                        memberType.ToDisplayString(),
                        member.ToDisplayString(GeneratorUtils.NameOnlyFormat));
                })
                .Where(member => member is not null)
                .Select(member => member!.Value)
                .ToImmutableArray()
        );
    }

    static void Execute(SourceProductionContext spc, ComponentDeclaration component)
    {
        spc.AddSource($"{component.Type}.g.cs",
            $$"""
            {{Templates.GeneratedFileHeader(typeof(ComponentGenerator))}}
            {{Templates.NamespaceDeclaration(component.Namespace)}}
            public static class {{component.Name}}Extensions
            {
            {{MemberExtensions(component.Members)}}
            }

            """);
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
}
