using Xunit.Abstractions;
namespace SourceGeneratorSupplement.Test;
public class WriterScopesTest
{
    static string Repeat(string str, int count)
    {
        return string.Join("", Enumerable.Repeat(str, count));
    }

    public WriterScopesTest(ITestOutputHelper helper)
    {
        this.helper = helper;
    }

    readonly ITestOutputHelper helper;

    [Fact]
    public void Block()
    {
        const int depth = 4;
        const string indent = "    ";
        var writer = new IndentedWriter(indent);

        using (var scope = new WriterBlockScope(writer, "open", "close", depth))
        {
            scope.Writer["//properly indented"].Line();
        }

        Assert.Equal(@$"open
{Repeat(indent, depth)}//properly indented
close
", writer.ToString());
    }

    [Fact]
    public void Indent()
    {
        const int depth = 4;
        const string indent = "    ";
        var writer = new IndentedWriter(indent);

        using (var scope = writer.IndentScope(depth))
        {
            scope.Writer["//properly indented"].End();
        }

        Assert.Equal(@$"{Repeat(indent, depth)}//properly indented", writer.ToString());
    }


    [Fact]
    public void DeclarationGlobalClass()
    {
        const string indent = "    ";
        var syntaxTree = TestHelper.CreateSyntaxTree(@"
class A
{
}");
        var compilation = TestHelper.CreateCompilation(syntaxTree);
        var writer = new IndentedWriter(indent);

        var symbol = compilation.GetTypeByMetadataName("A")!;

        using (var scope = writer.DeclarationScope(symbol))
        {
            scope.Writer["//class"].Line();
        }

        Assert.Equal(@"class A 
{
    //class
}
", writer.ToString());
    }

        [Fact]
        public void DeclarationNested()
        {
            const string indent = "    ";
            var syntaxTree = TestHelper.CreateSyntaxTree(@"
namespace X.Y
{
    partial class C
    {
        partial struct S
        {
        }
    }
}");
            var compilation = TestHelper.CreateCompilation(syntaxTree);
            var writer = new IndentedWriter(indent);

            var symbol = compilation.GetTypeByMetadataName("X.Y.C+S")!;

            using (var scope = writer.DeclarationScope(symbol))
            {
                scope.Writer["//nested"].Line();
            }

            Assert.Equal(@"namespace X.Y 
{
    partial class C 
    {
        partial struct S 
        {
            //nested
        }
    }
}
", writer.ToString());
        }

    [Fact]
    public void DeclarationNestedDepth()
    {
        const string indent = "    ";
        var syntaxTree = TestHelper.CreateSyntaxTree(@"
namespace X.Y
{
    partial class C
    {
        partial struct S
        {
        }
    }
}");

        var compilation = TestHelper.CreateCompilation(syntaxTree);
        var writer = new IndentedWriter(indent);
        var symbol = compilation.GetTypeByMetadataName("X.Y.C+S")!;
        using (var scope = writer.DeclarationScope(symbol, 2))
        {
            scope.Writer["//nested"].Line();
        }

        Assert.Equal(@"partial class C 
{
    partial struct S 
    {
        //nested
    }
}
", writer.ToString());
    }


    [Fact]
    public void DeclarationNestedTerminal()
    {
        const string indent = "    ";
        var syntaxTree = TestHelper.CreateSyntaxTree(@"
namespace X.Y
{
    partial class C
    {
        partial struct S
        {
        }
    }
}");

        var compilation = TestHelper.CreateCompilation(syntaxTree);
        var writer = new IndentedWriter(indent);
        var symbol = compilation.GetTypeByMetadataName("X.Y.C+S")!;
        var containingClassSymbol = symbol.ContainingType;
        using (writer.DeclarationScope(symbol, (symbol) => SymbolEqualityComparer.Default.Equals(symbol, containingClassSymbol)))
        {
            writer["//nested"].Line();
        }

        Assert.Equal(@"partial class C 
{
    partial struct S 
    {
        //nested
    }
}
", writer.ToString());
    }
}
