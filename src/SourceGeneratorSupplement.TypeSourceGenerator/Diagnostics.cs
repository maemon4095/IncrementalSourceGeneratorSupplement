using Microsoft.CodeAnalysis;
namespace SourceGeneratorSupplement.Generator;

static class Diagnostics
{
    static DiagnosticDescriptor ImplementationWasNotFoundDescripter { get; } = new DiagnosticDescriptor(
        id: "SGSG001",
        title: "ImplementationWasNotFound",
        messageFormat: "Implementation of {0} was not found.",
        category: "SourceGeneratorSupplement.Generator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static Diagnostic ImplementationWasNotFound(Location location, INamedTypeSymbol type)
    {
        return Diagnostic.Create(ImplementationWasNotFoundDescripter, location, type);
    }
}