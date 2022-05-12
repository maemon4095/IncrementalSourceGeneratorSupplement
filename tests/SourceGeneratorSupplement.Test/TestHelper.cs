namespace SourceGeneratorSupplement.Test;
static class TestHelper
{
    public static Compilation CreateCompilation(params SyntaxTree[] syntaxTrees)
    {
        return CSharpCompilation.Create(nameof(SourceFactoryTest), syntaxTrees, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
    }
    public static SyntaxTree CreateSyntaxTree(string source)
    {
        return CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
    }
}
