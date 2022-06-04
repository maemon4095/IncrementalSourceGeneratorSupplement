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


    static WriterDeclarationScope DeclarationScopeCore(IndentedWriter writer, ISymbol symbol, int depth)
    {
        return new(writer, symbol, depth);
    }
    static IndentedWriter DeclarationScopeCore(IndentedWriter writer, ISymbol symbol, int depth, Action action)
    {
        using (var scope = DeclarationScopeCore(writer, symbol, depth))
        {
            action();
        }
        return writer;
    }

    static WriterDeclarationScope DeclarationScopeCore(IndentedWriter writer, ISymbol symbol, Func<ISymbol, bool> terminal)
    {
        return new(writer, symbol, terminal);
    }
    static IndentedWriter DeclarationScopeCore(IndentedWriter writer, ISymbol symbol, Func<ISymbol, bool> terminal, Action action)
    {
        using (var scope = DeclarationScopeCore(writer, symbol, terminal))
        {
            action();
        }
        return writer;
    }


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol)
    {
        return DeclarationScopeCore(writer, symbol, -1);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, Action action)
    {
        return DeclarationScopeCore(writer, symbol, -1, action);
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol)
    {
        return DeclarationScopeCore(writer, symbol, -1);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, Action action)
    {
        return DeclarationScopeCore(writer, symbol, -1, action);
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol)
    {
        return DeclarationScopeCore(writer, symbol, -1);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, Action action)
    {
        return DeclarationScopeCore(writer, symbol, -1, action);
    }


    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, int depth)
    {
        return DeclarationScopeCore(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, IMethodSymbol symbol, int depth, Action action)
    {
        return DeclarationScopeCore(writer, symbol, depth, action);
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, int depth)
    {
        return DeclarationScopeCore(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamedTypeSymbol symbol, int depth, Action action)
    {
        return DeclarationScopeCore(writer, symbol, depth, action);
    }

    public static WriterDeclarationScope DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, int depth)
    {
        return DeclarationScopeCore(writer, symbol, depth);
    }
    public static IndentedWriter DeclarationScope(this IndentedWriter writer, INamespaceSymbol symbol, int depth, Action action)
    {
        return DeclarationScopeCore(writer, symbol, depth, action);
    }


    public static WriterDeclarationScope DeclarationScope(IndentedWriter writer, IMethodSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return DeclarationScopeCore(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(IndentedWriter writer, IMethodSymbol symbol, Func<ISymbol, bool> terminal, Action action)
    {
        return DeclarationScopeCore(writer, symbol, terminal, action);
    }

    public static WriterDeclarationScope DeclarationScope(IndentedWriter writer, INamedTypeSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return DeclarationScopeCore(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(IndentedWriter writer, INamedTypeSymbol symbol, Func<ISymbol, bool> terminal, Action action)
    {
        return DeclarationScopeCore(writer, symbol, terminal, action);
    }

    public static WriterDeclarationScope DeclarationScope(IndentedWriter writer, INamespaceSymbol symbol, Func<ISymbol, bool> terminal)
    {
        return DeclarationScopeCore(writer, symbol, terminal);
    }
    public static IndentedWriter DeclarationScope(IndentedWriter writer, INamespaceSymbol symbol, Func<ISymbol, bool> terminal, Action action)
    {
        return DeclarationScopeCore(writer, symbol, terminal, action);
    }
}