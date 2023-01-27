using Microsoft.CodeAnalysis;
using SourceGeneratorSupplement.Internal;
namespace SourceGeneratorSupplement.Generator;

[Generator]
public class TypeSourceGenerator : IIncrementalGenerator
{
    static string TypeSourceAttributeName { get; } = "TypeSourceAttribute";
    static string TypeSourceAttribute { get; } = $"{nameof(SourceGeneratorSupplement)}.{TypeSourceAttributeName}";

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
    [global::System.AttributeUsage(global::System.AttributeTargets.Method)]
    internal class {TypeSourceAttributeName} : global::System.Attribute
    {{
        public {TypeSourceAttributeName}(global::System.Type type, global::System.Int32 depthLimit = -1)
        {{
            this.Type = type;
            this.DepthLimit = depthLimit;
        }}

        public global::System.Type Type {{ get; }}
        public global::System.Int32 DepthLimit {{ get; }}
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
                return compilation.GetTypeByMetadataName(TypeSourceAttribute) ?? throw new NullReferenceException($"{TypeSourceAttributeName} was not found.");
            });

        var provider = context.SyntaxProvider.CreateAttribute(attributeSymbolProvider)
                .Select((pair, token) =>
                {
                    var (_, _, symbol, attributeSymbol, attributes) = pair;
                    var attributeData = attributes.First();
                    var args = attributeData.ConstructorArguments;
                    if (args.Length < 1) return default;
                    if (args[0].Value is not INamedTypeSymbol type) return default;
                    var depth = args.Select(a => a.Value).ElementAtOrDefault(1) as int? ?? 0;
                    var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
                    return (Symbol: (symbol as IMethodSymbol)!, Type: type, Depth: Math.Max(depth - 1, -1), Location: location);
                })
                .Where(pair => pair is { Symbol: not null, Type: not null })
                .Select((pair, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return new Model(pair.Symbol, pair.Type, pair.Depth, pair.Location);
                });

        context.RegisterSourceOutput(provider, this.ProductSource);
    }

    void ProductSource(SourceProductionContext context, Model model)
    {
        var refs = model.Type.DeclaringSyntaxReferences;
        if (refs.Length <= 0)
        {
            context.ReportDiagnostic(Diagnostics.ImplementationWasNotFound(model.Location, model.Type));
            return;
        }
        var writer = new IndentedWriter("    ");
        using (writer.DeclarationScope(model.Method))
        {
            writer["return @\""].End();
            var indentLevel = writer.IndentLevel;
            writer.IndentLevel = 0;
            foreach (var r in refs)
            {
                using (writer.DeclarationScope(model.Type.ContainingSymbol, model.DepthLimit))
                {
                    //remove trailing trivia because it does not affect indent level
                    var syntax = r.GetSyntax().WithoutTrailingTrivia();
                    var str = syntax.ToFullString();
                    var minindent = -1;

                    foreach (var line in str.EnumerateLines())
                    {
                        var indent = line.Length - line.TrimStart().Length;
                        //skip whitespace line
                        if (indent == line.Length) continue;
                        if ((uint)indent < (uint)minindent) minindent = indent;
                        if (minindent == 0) break;
                    }
                    if (minindent < 0) minindent = 0;

                    //leading trivia contains pragma directive so exclude it
                    foreach (var line in syntax.WithoutLeadingTrivia().ToFullString().EnumerateLines())
                    {
                        var trimmed = line.TrimStart();
                        var indent = line.Length - trimmed.Length;
                        writer[indent <= minindent ? trimmed : line.Slice(minindent)].Line();
                    }
                }
            }
            writer["\";"].Line();
            writer.IndentLevel = indentLevel;
        }

        context.AddSource($"{nameof(SourceGeneratorSupplement)}.{nameof(TypeSourceGenerator)}.{model.Method.ContainingType}.{model.Method.Name}.g.cs", writer.ToString());
    }

    readonly struct Model
    {
        public Model(IMethodSymbol method, INamedTypeSymbol type, int depthLimit, Location? location)
        {
            this.Method = method;
            this.Type = type;
            this.DepthLimit = depthLimit;
            this.Location = location ?? Location.None;
        }

        public Location Location { get; }
        public IMethodSymbol Method { get; }
        public INamedTypeSymbol Type { get; }
        public int DepthLimit { get; }
    }
}