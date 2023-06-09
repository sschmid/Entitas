using Microsoft.CodeAnalysis;

namespace Entitas.Generators;

public static class GeneratorUtils
{
    public static readonly SymbolDisplayFormat NameOnlyFormat = new SymbolDisplayFormat(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameOnly
    );

    public static INamedTypeSymbol? GetComponentInterfaceType(GeneratorSyntaxContext context)
    {
        return context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(IComponent).FullName);
    }
}
