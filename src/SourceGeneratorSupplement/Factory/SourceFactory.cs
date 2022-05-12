using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SourceGeneratorSupplement.Factory;

public static class SourceFactory
{
    static IEnumerable<SyntaxToken> GetModifiers(ISymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax()).OfType<MemberDeclarationSyntax>().SelectMany(s => s.Modifiers);
    }
    static bool IsPartial(ISymbol symbol)
    {
        return GetModifiers(symbol).Any(token => token.IsKind(SyntaxKind.PartialKeyword));
    }


    public static string GetDeclaration(ISymbol symbol)
    {
        switch (symbol)
        {
            case null:
                throw new ArgumentNullException(nameof(symbol));
            case INamespaceSymbol @namespace:
                return GetDeclaration(@namespace);
            case IMethodSymbol method:
                return GetDeclaration(method);
            case INamedTypeSymbol type:
                return GetDeclaration(type);
            default: return symbol.ToDisplayString(Format.TypeDeclaration);
        }
    }


    public static string GetDeclaration(INamedTypeSymbol symbol)
    {
        if (symbol is null) throw new ArgumentNullException(nameof(symbol));

        var builder = new StringBuilder();
        if (IsPartial(symbol))
        {
            //partial type declaration does not require same modifiers
            builder.Append(Keyword.Partial).Append(' ');

            if (symbol.IsRecord) builder.Append(Keyword.Record).Append(' ');
            if (symbol.IsRefLikeType) builder.Append(Keyword.Ref).Append(' ');

            builder.Append(symbol.TypeKind switch
            {
                TypeKind.Class => Keyword.Class,
                TypeKind.Struct => Keyword.Struct,
                TypeKind.Interface => Keyword.Interface,

                _ => throw new InvalidOperationException(),
            });
            builder.Append(' ')
                   .Append(symbol.ToDisplayString(Format.PartialTypeDeclaration));
        }
        else
        {
            if (symbol.IsStatic) builder.Append(Keyword.Static).Append(' ');
            if (symbol.IsReferenceType && symbol.IsSealed) builder.Append(Keyword.Sealed).Append(' ');
            if (symbol.IsAbstract) builder.Append(Keyword.Abstract).Append(' ');
            builder.Append(symbol.ToDisplayString(Format.TypeDeclaration));
        }

        return builder.Append(' ').ToString();
    }

    public static string GetDeclaration(IMethodSymbol symbol)
    {
        if (symbol is null) throw new ArgumentNullException(nameof(symbol));
        var builder = new StringBuilder();
        //currently ISymbol.ToDisplayString does not contain partial keyword.
        var decl = symbol.ToDisplayString(Format.MethodDeclaration);

        if (!IsPartial(symbol))
        {
            builder.Append(decl);
        }
        else
        {
            //extract modifers by diff to insert partial keyword before return type
            var nomod = symbol.ToDisplayString(Format.MethodDeclarationExcludeModifiers);
            var mods = decl.Substring(0, decl.Length - nomod.Length);

            builder.Append(mods).Append(Keyword.Partial).Append(' ').Append(nomod);
        }

        return builder.Append(' ').ToString();
    }

    public static string GetDeclaration(INamespaceSymbol symbol)
    {
        if (symbol is null) throw new ArgumentNullException(nameof(symbol));
        if (symbol.IsGlobalNamespace) return string.Empty;

        var builder = new StringBuilder();
        builder.Append(symbol.ToDisplayString(Format.NamespaceDeclaration)).Append(' ');
        return builder.ToString();
    }
}