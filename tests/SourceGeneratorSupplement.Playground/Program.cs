using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.Contracts;
using SourceGeneratorSupplement;

var syntaxTree = CreateSyntaxTree(@"
class G
{
}

namespace N.V
{
    interface IFoo
    {
        public void Z();
    }

    partial class A<T> : IFoo
        where T : global::System.Exception
    {
        public partial void Method<V>(int v) where V : struct;
        public static partial void M(int x, int y) => x + y;
        public static partial ref readonly int X(int y);
        public static partial void Y(ref int x, in int y, out int z);
        void IFoo.Z() { }
        public int P { get; set; }
    }

    abstract class Ab
    {
    }
    
    virtual class Vi
    {
    }

    enum E
    {
        A
    }
}");

var compilation = CreateCompilation(syntaxTree);
var writer = new IndentedWriter("    ");
var globalType = compilation.GetTypeByMetadataName("G");
writer.DeclarationScope(globalType, (writer, type) =>
{
    writer["//global"].Line();
});
var type = compilation.GetTypeByMetadataName("N.V.A`1")!;
writer.DeclarationScope(type, (writer, type) =>
{
    writer["//Comment"].Line();
});

foreach (var member in type.GetMembers())
{
    if (member.IsImplicitlyDeclared) continue;
    switch (member)
    {
        case INamedTypeSymbol t:
            writer.DeclarationScope(t, (writer, type) =>
            {
                writer["//Type"].Line();
            });
            break;
        case INamespaceSymbol n:
            writer.DeclarationScope(n, (writer, type) =>
            {
                writer["//Namespace"].Line();
            });
            break;

        case IMethodSymbol m:
            if (m.MethodKind != MethodKind.Ordinary || m.MethodKind == MethodKind.ExplicitInterfaceImplementation) break;
            writer.DeclarationScope(m, (writer, type) =>
            {
                writer["//Method"].Line();
            });
            break;
    }
}

writer.DeclarationScope(type, (writer, type) =>
{
    foreach (var member in type.GetMembers())
    {
        if (member.IsImplicitlyDeclared) continue;
        switch (member)
        {
            case INamedTypeSymbol t:
                writer.DeclarationScope(t, 0, (writer, type) =>
                {
                    writer["//Type"].Line();
                });
                break;
            case INamespaceSymbol n:
                writer.DeclarationScope(n, 0, (writer, type) =>
                {
                    writer["//Namespace"].Line();
                });
                break;

            case IMethodSymbol m:
                if (m.MethodKind != MethodKind.Ordinary) break;
                writer.DeclarationScope(m, 0, (writer, type) =>
                {
                    writer["//Method"].Line();
                });
                break;
        }
    }
});



Console.WriteLine(writer);

Console.ReadLine();

[Pure]
static Compilation CreateCompilation(params SyntaxTree[] syntaxTrees)
{
    return CSharpCompilation.Create("Playground", syntaxTrees, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
}
[Pure]
static SyntaxTree CreateSyntaxTree(string source)
{
    return CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
}