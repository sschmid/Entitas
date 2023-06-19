using Microsoft.CodeAnalysis;

namespace Entitas.Generators;

public class EntitasDiagnostics
{
    public static void ReportDiagnostics(SourceProductionContext sourceProductionContext, Diagnostic diagnostic)
        => sourceProductionContext.ReportDiagnostic(diagnostic);

    public static readonly DiagnosticDescriptor CouldNotFindInterface = new(id: "ENTITASGEN001",
        title: "Couldn't find necessary Interface",
        messageFormat: "Please make sure the compilation includes the '{0}' Interface.",
        category: "Prerequisites",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NamedTypeSymbolNotFound = new(id: "ENTITASGEN002",
        title: "NamedTypeSymbol was not found in compilation",
        messageFormat: "Error finding the type '{0}' in the compilation. This should not occur.",
        category: "ComponentGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
