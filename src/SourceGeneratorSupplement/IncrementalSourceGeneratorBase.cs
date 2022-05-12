using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace SourceGeneratorSupplement;

public abstract class IncrementalSourceGeneratorBase<TSyntax, TBundle> : IIncrementalGenerator
    where TSyntax : MemberDeclarationSyntax
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(this.AddInitialSource);
        var syntaxProvider = this.CreateSyntaxProvider(context);
        context.RegisterSourceOutput(syntaxProvider, this.AddSource);
    }
    IncrementalValuesProvider<TBundle> CreateSyntaxProvider(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider.CreateSyntaxProvider(
             static (node, token) =>
             {
                 token.ThrowIfCancellationRequested();
                 return node is TSyntax { AttributeLists.Count: > 0 };
             },
             static (context, token) =>
             {
                 token.ThrowIfCancellationRequested();
                 var syntax = (TSyntax)context.Node;
                 if (context.SemanticModel.GetDeclaredSymbol(syntax, token) is not INamedTypeSymbol symbol) return default;
                 var attributes = symbol.GetAttributes();
                 return (symbol, attributes);
             })
             .Where(tuple => tuple != default).Combine(context.CompilationProvider)
             .Select((tuple, token) =>
             {
                 token.ThrowIfCancellationRequested();
                 var ((symbol, allAttributes), compilation) = tuple;
                 var result = this.CreateContext(compilation, allAttributes, out var context);
                 if (!result) return default;
                 return context;
             })
             .WithComparer(this.GetBundleEqualityComparer());
    }

    void AddInitialSource(IncrementalGeneratorPostInitializationContext context)
    {
        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        try
        {
            this.ProductInitialSource(context);
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.GetType()} was thrown when generating initial source. StackTrace : {ex.StackTrace}", ex);
        }
    }
    void AddSource(SourceProductionContext context, TBundle bundle)
    {
        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        try
        {
            this.ProductSource(context, bundle);
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.GetType()} was thrown when generating source. StackTrace : {ex.StackTrace}", ex);
        }
    }

    protected virtual IEqualityComparer<TBundle> GetBundleEqualityComparer() => EqualityComparer<TBundle>.Default;
    protected abstract bool CreateContext(Compilation compilation, ImmutableArray<AttributeData> attributes, out TBundle context);
    protected abstract void ProductInitialSource(IncrementalGeneratorPostInitializationContext context);
    protected abstract void ProductSource(SourceProductionContext context, TBundle bundle);
}