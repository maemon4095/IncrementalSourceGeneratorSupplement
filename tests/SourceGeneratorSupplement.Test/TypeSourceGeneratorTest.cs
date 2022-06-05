using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceGeneratorRunner;

namespace SourceGeneratorSupplement.Test;
public class TypeSourceGeneratorTest
{
    public TypeSourceGeneratorTest(ITestOutputHelper helper)
    {
        this.helper = helper;
        this.runner = GeneratorRunner.Create(RunnerConfig.Default, () => new Generator.TypeSourceGenerator());
    }

    readonly ITestOutputHelper helper;
    readonly GeneratorRunner runner;

    [Fact]
    public void Global_Class()
    {
        var source = @"
using SourceGeneratorSupplement;

    public class A
    {
        public static void Method()
        {

        }
    }


public partial class Source
{
    [TypeSource(typeof(A))]
    public static partial string A();
}
";
        var result = this.runner.Run(source);
        var generated = result.GeneratedSources.First(s => s.HintName == "Source.Source.A.g.cs");
        var generatedSource = generated.SourceText.ToString();


        Assert.Equal(@"partial class Source 
{
    public static partial global::System.String A() 
    {
        return @""public class A
{
    public static void Method()
    {

    }
}
"";
    }
}
", generatedSource);
    }



    [Fact]
    public void Class()
    {
        var source = @"
using SourceGeneratorSupplement;

namespace N
{
    public class A
    {
        public static void Method()
        {

        }
    }
}

public partial class Source
{
    [TypeSource(typeof(N.A), 1)]
    public static partial string A();
}
";
        var result = this.runner.Run(source);
        var generated = result.GeneratedSources.First(s => s.HintName == "Source.Source.A.g.cs");
        var generatedSource = generated.SourceText.ToString();

        helper.WriteLine($"//{generated.HintName}");
        helper.WriteLine(generatedSource);

        Assert.Equal(@"partial class Source 
{
    public static partial global::System.String A() 
    {
        return @""public class A
{
    public static void Method()
    {

    }
}
"";
    }
}
", generatedSource);
    }
}
