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
    public sealed partial class ContextGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var options = initContext.AnalyzerConfigOptionsProvider;

            var contextChanged = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextCandidate, CreateContextDeclaration)
                .Where(static context => context is not null)
                .Select(static (context, _) => context!.Value);

            initContext.RegisterSourceOutput(contextChanged.Combine(options), OnContextChanged);
        }

        static bool IsContextCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.BaseList.Types.Any(static baseType => baseType.Type switch
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

            var componentInterface = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IContext");
            if (componentInterface is null)
                return null;

            var isContext = symbol.Interfaces.Contains(componentInterface);
            if (!isContext)
                return null;

            return new ContextDeclaration(candidate, symbol);
        }

        static void OnContextChanged(SourceProductionContext spc, (ContextDeclaration Context, AnalyzerConfigOptionsProvider OptionsProvider) pair)
        {
            var (context, optionsProvider) = pair;
            ComponentIndex(spc, context, optionsProvider);
            Entity(spc, context, optionsProvider);
            Matcher(spc, context, optionsProvider);
            Context(spc, context, optionsProvider);
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
