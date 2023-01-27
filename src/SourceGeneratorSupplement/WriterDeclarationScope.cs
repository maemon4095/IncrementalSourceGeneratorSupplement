using Microsoft.CodeAnalysis;
using SourceGeneratorSupplement.Factory;

namespace SourceGeneratorSupplement;
public struct WriterDeclarationScope : IDisposable
{
    public WriterDeclarationScope(IndentedWriter writer, ISymbol? symbol, int depth, Func<ISymbol, bool>? terminal)
    {
        this.Writer = writer;
        this.depth = InitialWrite(writer, symbol, depth, terminal);
    }
    static int InitialWrite(IndentedWriter writer, ISymbol? symbol, int depthLimit, Func<ISymbol, bool>? terminal)
    {
        if (symbol is null) return 0;
        var depth = 0;
        foreach (var containing in ContainingsAndSelf(symbol, depthLimit, terminal))
        {
            writer[SourceFactory.GetDeclaration(containing)].Line()
                  ['{'].Line().Indent(1);

            depth++;
        }
        return depth;
    }

    static IEnumerable<ISymbol> ContainingsAndSelf(ISymbol symbol, int depth, Func<ISymbol, bool>? terminal)
    {
        if (depth == 0) yield break;
        if (symbol is INamespaceSymbol { IsGlobalNamespace: true }) yield break;
        var reversed = ReversedContainings(symbol);
        var taken = depth < 0 ? reversed : reversed.Take(depth - 1);
        var containings = (terminal is null ? taken : Until(taken, terminal)).Reverse();

        foreach (var containing in containings)
        {
            yield return containing;
        }
        yield return symbol;

        static IEnumerable<T> Until<T>(IEnumerable<T> seq, Func<T, bool> pred)
        {
            foreach (var item in seq)
            {
                yield return item;
                if (pred(item)) yield break;
            }
        }
    }
    static IEnumerable<ISymbol> ReversedContainings(ISymbol symbol)
    {
        var current = symbol.ContainingType;
        while (current is not null)
        {
            yield return current;
            current = current.ContainingType;
        }
        if (symbol.ContainingNamespace is not null && !symbol.ContainingNamespace.IsGlobalNamespace)
            yield return symbol.ContainingNamespace;
    }

    public IndentedWriter Writer { get; }
    readonly int depth;

    public void Dispose()
    {
        var writer = this.Writer;
        for (var i = 0; i < this.depth; ++i)
        {
            writer.Indent(-1)['}'].Line();
        }
    }
}
