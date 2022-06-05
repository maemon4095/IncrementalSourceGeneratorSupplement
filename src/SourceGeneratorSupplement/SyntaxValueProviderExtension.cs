using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace SourceGeneratorSupplement;
public static class SyntaxValueProviderExtension
{
    public static IncrementalValuesProvider<(T Node, SemanticModel SemanticModel)> CreateOfType<T>(this SyntaxValueProvider provider)
        where T : SyntaxNode
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

    public static IncrementalValuesProvider<(MemberDeclarationSyntax Node, SemanticModel SemanticModel, ISymbol Symbol, INamedTypeSymbol AttributeSymbol, ImmutableArray<AttributeData> Attributes)> CreateAttribute(this SyntaxValueProvider provider, IncrementalValueProvider<INamedTypeSymbol> attributeProvider)
    {
        return provider.CreateSyntaxProvider(
            static (node, token) =>
            {
                token.ThrowIfCancellationRequested();
                return node is MemberDeclarationSyntax { AttributeLists.Count: > 0 };
            },
            static (context, token) =>
            {
                token.ThrowIfCancellationRequested();
                return ((context.Node as MemberDeclarationSyntax)!, context.SemanticModel);
            })
            .Combine(attributeProvider)
            .Select(static (pair, token) =>
            {
                token.ThrowIfCancellationRequested();
                var ((node, model), attributeSymbol) = pair;
                var symbol = model.GetDeclaredSymbol(node);
                if (symbol is null) return default;
                var attributes = symbol.GetAttributes().Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol));
                if (!attributes.Any()) return default;

                return (node, model, symbol, attributeSymbol, attributes.ToImmutableArray());
            })
            .Where(pair => pair.node is not null);
    }
}
