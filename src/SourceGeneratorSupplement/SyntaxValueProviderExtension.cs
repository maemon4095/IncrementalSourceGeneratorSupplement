using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace SourceGeneratorSupplement;
public static class SyntaxValueProviderExtension
{
    public static IncrementalValuesProvider<(T Node, SemanticModel SemanticModel)> CreateOfType<T>(this SyntaxValueProvider provider)
        where T : CSharpSyntaxNode
    {
        return provider.CreateSyntaxProvider(
            static (node, token) =>
            {
                token.ThrowIfCancellationRequested();
                return node is T;
            },
            static (context, token) =>
            {
                token.ThrowIfCancellationRequested();
                return ((context.Node as T)!, context.SemanticModel);
            });
    }

    public static IncrementalValuesProvider<(T Node, SemanticModel SemanticModel, INamedTypeSymbol Symbol, INamedTypeSymbol AttributeSymbol, ImmutableArray<AttributeData> Attributes)> CreateAttribute<T>(this SyntaxValueProvider provider, IncrementalValueProvider<INamedTypeSymbol> attributeProvider)
        where T : BaseTypeDeclarationSyntax
    {
        return provider.CreateSyntaxProvider(
            static (node, token) =>
            {
                token.ThrowIfCancellationRequested();
                return node is T { AttributeLists.Count: > 0 };
            },
            static (context, token) =>
            {
                token.ThrowIfCancellationRequested();
                return ((context.Node as T)!, context.SemanticModel);
            })
            .Combine(attributeProvider)
            .Select(static (pair, token) =>
            {
                token.ThrowIfCancellationRequested();
                var ((node, model), attributeSymbol) = pair;
                var symbol = model.GetDeclaredSymbol(node);
                if (symbol is null) return default;
                var attributes = symbol.GetAttributes();

                if (!attributes.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))) return default;
                return (node, model, symbol, attributeSymbol, attributes.ToImmutableArray());
            })
            .Where(pair => pair.node is not null);
    }
}
