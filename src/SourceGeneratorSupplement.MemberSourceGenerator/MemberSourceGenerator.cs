using SourceGeneratorSupplement;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace SourceGeneratorSupplement.MemberSourceGenerator;

[Generator]
public class MemberSourceGenerator : IIncrementalGenerator
{
    static string MemberSourceAttributeName { get; } = "MemberSourceAttribute";
    static string MemberSourceAttribute { get; } = $"{nameof(SourceGeneratorSupplement)}.{MemberSourceAttributeName}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(this.ProductInitialSource);
        this.RegisterGenerator(context);
    }

    void ProductInitialSource(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("MemberSourceAttribute", @$"
namespace {nameof(SourceGeneratorSupplement)}
{{
    [AttributeUsage(AttributeTargets.Method)]
    internal class {MemberSourceAttributeName} : Attribute
    {{
        public MemberSourceAttribute(Type type, string path = """")
        {{
            this.Type = type;
            this.Path = path;
        }}

        public Type Type {{ get; }}
        public string Path {{ get; }}
    }}
}}
");
    }

    void RegisterGenerator(IncrementalGeneratorInitializationContext context)
    {
        var attributeSymbolProvider = context.CompilationProvider.Select(
            (compilation, token) =>
            {
                token.ThrowIfCancellationRequested();
                return compilation.GetTypeByMetadataName(MemberSourceAttribute) ?? throw new NullReferenceException($"{MemberSourceAttributeName} was not found.");
            });

        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, token) =>
            {
                token.ThrowIfCancellationRequested();
                return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
            },
            (context, token) =>
            {
                var node = (context.Node as MethodDeclarationSyntax)!;
                if (context.SemanticModel.GetDeclaredSymbol(node) is not IMethodSymbol symbol) return default;
                return (symbol, context.SemanticModel);
            })
            .Where(pair => pair.symbol is not null)
            .Combine(attributeSymbolProvider)
            .Select((pair, token) =>
            {
                var ((symbol, semanticModel), attributeSymbol) = pair;
                var attributeData = symbol.GetAttributes().FirstOrDefault(d => SymbolEqualityComparer.Default.Equals(d.AttributeClass, attributeSymbol));
                if (attributeData is null) return default;
                var (type, path) = parseArgs(attributeData.ConstructorArguments);
                if (type is null) return default;
                return (symbol, type, path);
            })
            .Where(pair => pair.symbol is not null)
            .Select((pair, token) =>
            {
                token.ThrowIfCancellationRequested();
                return new Bundle(pair.symbol, pair.type, pair.path);
            });

        context.RegisterSourceOutput(provider, this.ProductSource);

        static (INamedTypeSymbol, string) parseArgs(ImmutableArray<TypedConstant> args)
        {
            var type = default(INamedTypeSymbol);
            var path = string.Empty;

            var enumerator = args.GetEnumerator();

            if (!enumerator.MoveNext()) return default;

            type = enumerator.Current.Value as INamedTypeSymbol;
            if (type is null) return default;

            if (enumerator.MoveNext())
            {
                path = enumerator.Current.Value as string ?? string.Empty;
            }

            return (type, path);
        }
    }

    void ProductSource(SourceProductionContext context, Bundle bundle)
    {
        var writer = new IndentedWriter("    ");
        writer.DeclarationScope(bundle.Method, () =>
        {
            
        });
    }

    readonly struct Bundle
    {
        public Bundle(IMethodSymbol method, ITypeSymbol type, string path)
        {
            this.Method = method;
            this.Type = type;
            this.Path = path;
        }

        public IMethodSymbol Method { get; }
        public ITypeSymbol Type { get; }
        public string Path { get; }
    }
}