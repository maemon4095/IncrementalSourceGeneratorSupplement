using Microsoft.CodeAnalysis;
using SourceGeneratorSupplement.Factory;

namespace SourceGeneratorSupplement;
public struct WriterDeclarationScope : IDisposable
{
    public WriterDeclarationScope(IndentedWriter writer, ISymbol symbol, int depth)
    {
        this.Writer = writer;
        this.depth = InitialWrite(writer, depth, symbol);
    }
    public WriterDeclarationScope(IndentedWriter writer, ISymbol symbol, Func<ISymbol, bool> terminal)
    {
        this.Writer = writer;
        this.depth = InitialWrite(writer, terminal, symbol);
    }

    static int InitialWrite(IndentedWriter writer, int depthLimit, ISymbol symbol)
    {
        var depth = 0;
        foreach (var containing in ContainingsAndSelf(symbol, depthLimit))
        {
            writer[SourceFactory.GetDeclaration(containing)].Line()
                  ['{'].Line().Indent(1);

            depth++;
        }
        return depth;
    }
    static int InitialWrite(IndentedWriter writer, Func<ISymbol, bool> terminal, ISymbol symbol)
    {
        var depth = 0;
        foreach (var containing in ContainingsAndSelf(symbol, terminal))
        {
            writer[SourceFactory.GetDeclaration(containing)].Line()
                  ['{'].Line().Indent(1);

            depth++;
        }
        return depth;
    }

    static IEnumerable<ISymbol> ContainingsAndSelf(ISymbol symbol, int depth)
    {
        var reversed = ReversedContainings(symbol);
        var containings = (depth < 0 ? reversed : reversed.Take(depth)).Reverse();

        foreach (var containing in containings)
        {
            yield return containing;
        }
        yield return symbol;
    }

    static IEnumerable<ISymbol> ContainingsAndSelf(ISymbol symbol, Func<ISymbol, bool> terminal)
    {
        var containings = Until(ReversedContainings(symbol), terminal).Reverse();

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
        if (!symbol.ContainingNamespace.IsGlobalNamespace)
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
