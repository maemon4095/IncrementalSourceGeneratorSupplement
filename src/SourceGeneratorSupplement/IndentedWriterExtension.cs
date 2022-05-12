using Microsoft.CodeAnalysis;

namespace SourceGeneratorSupplement;

public static class IndentedWriterExtension
{
    public static WriterIndentScope IndentScope(this IndentedWriter writer, int level)
    {
        return new(writer, level);
    }
    public static IndentedWriter IndentScope(this IndentedWriter writer, int level, Action<IndentedWriter> action)
    {
        using (var scope = writer.IndentScope(level))
        {
            action(scope.Writer);
        }
        return writer;
    }
 
    public static WriterBlockScope BlockScope(this IndentedWriter writer, int level)
    {
        return new(writer, "{", "}", level);
    }
    public static IndentedWriter BlockScope(this IndentedWriter writer, int level, Action<IndentedWriter> action)
    {
        using (var scope = writer.BlockScope(level))
        {
            action(scope.Writer);
        }
        return writer;
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, ISymbol symbol)
    {
        return new(writer, symbol);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, ISymbol symbol, Action<IndentedWriter, ISymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
 
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, ISymbol symbol, Func<ISymbol, bool> terminal)
    {
        return new(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, ISymbol symbol, Func<ISymbol, bool> terminal, Action<IndentedWriter, ISymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, terminal))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
}