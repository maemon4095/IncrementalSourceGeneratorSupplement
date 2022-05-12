using SourceGeneratorSupplement.Factory;
namespace SourceGeneratorSupplement.Test;
using static Test.TestHelper;

public class SourceFactoryTest
{
    [Fact]
    public void GetDeclaration_PartialClass()
    {
        var syntaxTree = CreateSyntaxTree(@"
partial class A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial class A ", decl);
    }

    [Fact]
    public void GetDeclaration_PartialStruct()
    {
        var syntaxTree = CreateSyntaxTree(@"
partial struct A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial struct A ", decl);
    }

    [Fact]
    public void GetDeclaration_PartialInterface()
    {
        var syntaxTree = CreateSyntaxTree(@"
partial interface A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial interface A ", decl);
    }

    [Fact]
    public void GetDeclaration_SealedPartialClass()
    {
        var syntaxTree = CreateSyntaxTree(@"
sealed partial class A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial class A ", decl);
    }

    [Fact]
    public void GetDeclaration_SealedClass()
    {
        var syntaxTree = CreateSyntaxTree(@"
sealed class A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("sealed class A ", decl);
    }

    [Fact]
    public void GetDeclaration_PartialReadOnlyStruct()
    {
        var syntaxTree = CreateSyntaxTree(@"
readonly partial struct A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial struct A ", decl);
    }

    [Fact]
    public void GetDeclaration_ReadOnlyStruct()
    {
        var syntaxTree = CreateSyntaxTree(@"
readonly struct A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("readonly struct A ", decl);
    }

    [Fact]
    public void GetDeclaration_Record()
    {
        var syntaxTree = CreateSyntaxTree(@"
record A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("record A ", decl);
    }


    [Fact]
    public void GetDeclaration_RecordStruct()
    {
        var syntaxTree = CreateSyntaxTree(@"
record struct A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("record struct A ", decl);
    }

    [Fact]
    public void GetDeclaration_PartialRecord()
    {
        var syntaxTree = CreateSyntaxTree(@"
partial record A(int X)
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("partial record class A ", decl);
    }


    [Fact]
    public void GetDeclaration_RecordWithPrimaryCtor()
    {
        var syntaxTree = CreateSyntaxTree(@"
record A(int x)
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("record A ", decl);
    }

    [Fact]
    public void GetDeclaration_Class()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("class A ", decl);
    }

    [Fact]
    public void GetDeclaration_StaticClass()
    {
        var syntaxTree = CreateSyntaxTree(@"
static class A
{
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("static class A ", decl);
    }

    [Fact]
    public void GetDeclaration_ClassInNamespace()
    {
        var syntaxTree = CreateSyntaxTree(@"
namespace Containing
{
    class A {}
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("Containing.A");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("class A ", decl);
    }

    [Fact]
    public void GetDeclaration_NestedClass()
    {
        var syntaxTree = CreateSyntaxTree(@"
class Containing
{
    class Nested {}
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("Containing+Nested");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("class Nested ", decl);
    }

    [Fact]
    public void GetDeclaration_NestedNamespace()
    {
        var syntaxTree = CreateSyntaxTree(@"
namespace Containing.Nested
{
}");

        var compilation = CreateCompilation(syntaxTree);
        var containingSymbol = compilation.GlobalNamespace.GetNamespaceMembers().First(n => n.Name == "Containing");
        var containingDecl = SourceFactory.GetDeclaration(containingSymbol);

        Assert.Equal("namespace Containing ", containingDecl);

        var nestedSymbol = containingSymbol.GetNamespaceMembers().First(n => n.Name == "Nested");
        var nestedDecl = SourceFactory.GetDeclaration(nestedSymbol);

        Assert.Equal("namespace Containing.Nested ", nestedDecl);
    }


    [Fact]
    public void GetDeclaration_NamespaceContainingNested()
    {
        var syntaxTree = CreateSyntaxTree(@"
namespace Namespace
{
    class Containing
    {
        class Nested {}
    }
}");

        var compilation = CreateCompilation(syntaxTree);

        var symbol = compilation.GetTypeByMetadataName("Namespace.Containing+Nested");

        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("class Nested ", decl);
    }

    [Fact]
    public void GetDeclaration_Generic()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A<T>
    where T : class
{
    
}");

        var compilation = CreateCompilation(syntaxTree);
        var symbol = compilation.GetTypeByMetadataName("A`1");
        var decl = SourceFactory.GetDeclaration(symbol);

        Assert.Equal("class A<T> ", decl);
    }

    [Fact]
    public void GetDeclaration_Method()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    int Method(long arg) { return 0; }
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("Method").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private global::System.Int32 Method(global::System.Int64 arg) ", decl);
    }

    [Fact]
    public void GetDeclaration_GenericMethod()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    T Method<T>(T arg) where T : struct
    {
        return arg; 
    }
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("Method").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private T Method<T>(T arg) ", decl);
    }

    [Fact]
    public void GetDeclaration_ExtensionMethod()
    {
        var syntaxTree = CreateSyntaxTree(@"
static partial class A
{
    static partial int Ext(this int x);
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("Ext").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private static partial global::System.Int32 Ext(this global::System.Int32 x) ", decl);
    }

    [Fact]
    public void GetDeclaration_RefReturn()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    partial ref int X();
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("X").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private partial ref global::System.Int32 X() ", decl);
    }

    [Fact]
    public void GetDeclaration_RefReadonlyReturn()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    partial ref readonly int X();
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("X").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private partial ref readonly global::System.Int32 X() ", decl);
    }

    [Fact]
    public void GetDeclaration_RefArg()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    partial void X(ref int x);
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("X").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private partial void X(ref global::System.Int32 x) ", decl);
    }

    [Fact]
    public void GetDeclaration_InArg()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    partial void X(in int x);
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("X").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private partial void X(in global::System.Int32 x) ", decl);
    }

    [Fact]
    public void GetDeclaration_OutArg()
    {
        var syntaxTree = CreateSyntaxTree(@"
class A
{
    partial void X(out int x);
}");

        var compilation = CreateCompilation(syntaxTree);
        var classSymbol = compilation.GetTypeByMetadataName("A")!;
        var methodSymbol = classSymbol.GetMembers("X").OfType<IMethodSymbol>().First();
        var decl = SourceFactory.GetDeclaration(methodSymbol);

        Assert.Equal(@"private partial void X(out global::System.Int32 x) ", decl);
    }
}