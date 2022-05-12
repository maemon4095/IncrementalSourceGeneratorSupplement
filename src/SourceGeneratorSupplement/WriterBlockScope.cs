namespace SourceGeneratorSupplement;

public readonly struct WriterBlockScope : IDisposable
{
    public WriterBlockScope(IndentedWriter writer, string open, string close, int level)
    {
        this.Writer =  writer;
        this.Level = level;
        this.close = close;

        writer[open].Line().Indent(level);
    }

    public IndentedWriter Writer { get; }
    public int Level { get;}
    readonly string close;

    public void Dispose()
    {
        this.Writer.Indent(-this.Level)[this.close].Line();
    }
}