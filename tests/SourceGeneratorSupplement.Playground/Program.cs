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

        public class I
        {
        }
    }

    abstract class Ab
    {
    }

    enum E
    {
        A
    }
}");

var root = syntaxTree.GetCompilationUnitRoot();


Console.WriteLine(Source.D());

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



partial class Source
{
    [TypeSource(typeof(A))]
    public static partial string A();
    [TypeSource(typeof(B))]
    public static partial string B();   
    [TypeSource(typeof(B.C))]
    public static partial string C();
    [TypeSource(typeof(B.D), 1)]
    public static partial string D();
    [TypeSource(typeof(int))]
    public static partial string E();
}


class A
{
    public string Text { get; }
}

class B
{
    public class C
    {
        public static int StaticMethod() => 0;
        public void Method() { }
    }

    public class D
    {

    }
}