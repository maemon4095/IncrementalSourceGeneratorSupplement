using System.Text;
using SourceGeneratorSupplement.Internal;

namespace SourceGeneratorSupplement;

public class IndentedWriter : TextWriter
{
    static void WriteIndent(StringBuilder builder, string indent, int level)
    {
        while (level > 0)
        {
            builder.Append(indent);
            level--;
        }
    }

    public IndentedWriter(StringBuilder builder, string indent, string newLine)
    {
        this.builder = builder;
        this.tabPending = true;
        this.IndentString = indent;
        this.CoreNewLine = newLine.ToCharArray();
        this.IndentLevel = 0;
        this.detector = new NewLineDetector();
    }
    public IndentedWriter(string indent)
        : this(indent, Environment.NewLine)
    { }

    public IndentedWriter(string indent, string newLine)
        : this(new StringBuilder(), indent, newLine)
    {

    }

    readonly StringBuilder builder;
    bool tabPending;
    NewLineDetector detector;
    public int IndentLevel { get; set; }
    public string IndentString { get; }

    public override Encoding Encoding => Encoding.Unicode;

    public unsafe IndentedWriter this[ReadOnlySpan<char> span]
    {
        get
        {
            this.Write(span);
            return this;
        }
    }

    public IndentedWriter this[string str] => this[str.AsSpan()];

    public IndentedWriter this[char chara]
    {
        get
        {
            this.Write(chara);
            return this;
        }
    }

    public IndentedWriter this[object obj] => this[obj.ToString()];

    public override void Write(char chara)
    {
        if (this.tabPending) this.FlushIndent();
        if (!this.detector.Transition(chara))
        {
            switch (this.detector.Category)
            {
                case NewLineCategory.None: break;
                case NewLineCategory.CR: this.FlushIndent(); break;
                default: this.tabPending = true; break;
            }
            this.detector.Reset();
        }
        this.builder.Append(chara);
    }

    public override void Write(string value)
    {
        this.Write(value.AsSpan());
    }
    public override void Write(char[] buffer) => this.Write(buffer.AsSpan());
    public override void Write(char[] buffer, int index, int count) => this.Write(buffer.AsSpan(index, count));

    public void Write(ReadOnlySpan<char> span)
    {
        var builder = this.builder;
        var tabPending = this.tabPending;
        var detector = this.detector;
        var level = this.IndentLevel;
        var indent = this.IndentString;
        foreach (var chara in span)
        {
            if (tabPending) { WriteIndent(builder, indent, level); tabPending = false; }
            if (!detector.Transition(chara))
            {
                switch (detector.Category)
                {
                    case NewLineCategory.None: break;
                    case NewLineCategory.CR: this.FlushIndent(); break;
                    default: tabPending = true; break;
                }
                detector.Reset();
            }
            builder.Append(chara);
        }
        this.tabPending = tabPending;
        this.detector = detector;
    }

    public IndentedWriter Line()
    {
        this.builder.Append(this.CoreNewLine);
        this.tabPending = true;
        return this;
    }
    public void End()
    {
        switch (this.detector.Category)
        {
            case NewLineCategory.CR:
                this.tabPending = true;
                this.detector.Reset();
                break;
        }
    }

    public IndentedWriter Indent(int delta)
    {
        this.IndentLevel += delta;
        return this;
    }
    void FlushIndent()
    {
        WriteIndent(this.builder, this.IndentString, this.IndentLevel);
        this.detector.Reset();
        this.tabPending = false;
    }

    public override string ToString() => this.builder.ToString();
}
