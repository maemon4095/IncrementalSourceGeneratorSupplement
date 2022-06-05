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


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, ISymbol? symbol, int depth = -1, Func<ISymbol, bool>? terminal = null)
    {
        return new(writer, symbol, depth, terminal);
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, ISymbol? symbol, Func<ISymbol, bool>? terminal)
    {
        return DeclarationScope(writer, symbol, -1, terminal);
    }
}