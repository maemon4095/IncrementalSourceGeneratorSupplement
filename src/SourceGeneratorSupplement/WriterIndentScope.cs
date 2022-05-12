namespace SourceGeneratorSupplement;

public readonly struct WriterIndentScope : IDisposable
{
    public WriterIndentScope(IndentedWriter writer, int level)
    {
        this.Writer = writer;
        this.Level = level;
        writer.Indent(level);
    }
    
    public IndentedWriter Writer { get; }
    public int Level { get; }

    public void Dispose()
    {
        this.Writer.Indent(-this.Level);
    }
}
