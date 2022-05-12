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


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol)
    {
        return new(writer, symbol);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, Action<IndentedWriter, INamedTypeSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol)
    {
        return new(writer, symbol);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, Action<IndentedWriter, INamespaceSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol)
    {
        return new(writer, symbol);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, Action<IndentedWriter, IMethodSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, int depth)
    {
        return new(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, int depth, Action<IndentedWriter, INamedTypeSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, depth))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, int depth)
    {
        return new(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, int depth, Action<IndentedWriter, INamespaceSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, depth))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, int depth)
    {
        return new(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, int depth, Action<IndentedWriter, IMethodSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, depth))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return new(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, Func<ISymbol, bool> terminal, Action<IndentedWriter, INamedTypeSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, terminal))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return new(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, Func<ISymbol, bool> terminal, Action<IndentedWriter, INamespaceSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, terminal))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return new(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, Func<ISymbol, bool> terminal, Action<IndentedWriter, IMethodSymbol> action)
    {
        using (var scope = writer.DeclarationScope(symbol, terminal))
        {
            action(scope.Writer, symbol);
        }
        return writer;
    }
}