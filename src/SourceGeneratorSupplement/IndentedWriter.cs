using System.Text;

namespace SourceGeneratorSupplement;
public class IndentedWriter : TextWriter
{
    enum NewLineCategory
    {
        None, CR, LF, CRLF
    }
    struct NewLineDeterminator
    {

        public NewLineDeterminator()
        {
            this.Category = NewLineCategory.None;
        }

        public NewLineCategory Category { get; private set; }


        public bool Transition(char chara)
        {
            switch (this.Category)
            {
                case NewLineCategory.CR:
                    switch (chara)
                    {
                        case '\n':
                            this.Category = NewLineCategory.CRLF;
                            return false;
                        default:
                            this.Category = NewLineCategory.CR;
                            return false;
                    }
                default:
                    switch (chara)
                    {
                        case '\r':
                            this.Category = NewLineCategory.CR;
                            return true;
                        case '\n':
                            this.Category = NewLineCategory.LF;
                            return false;
                        default:
                            this.Category = NewLineCategory.None;
                            return false;
                    }
            }
        }

        public void Reset()
        {
            this.Category = NewLineCategory.None;
        }
    }

    static void WriteIndent(StringBuilder builder, string indent, int level)
    {
        for (; level > 0; --level)
        {
            builder.Append(indent);
        }
    }

    public IndentedWriter(StringBuilder builder, string indent, string newLine)
    {
        this.builder = builder;
        this.tabPending = true;
        this.IndentString = indent;
        this.CoreNewLine = newLine.ToCharArray();
        this.IndentLevel = 0;
        this.determinator = new NewLineDeterminator();
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
    NewLineDeterminator determinator;
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
        if (!this.determinator.Transition(chara))
        {
            switch (this.determinator.Category)
            {
                case NewLineCategory.None: break;
                case NewLineCategory.CR: this.FlushIndent(); break;
                default: this.tabPending = true; break;
            }
            this.determinator.Reset();
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
        var determinator = this.determinator;
        var level = this.IndentLevel;
        var indent = this.IndentString;
        foreach (var chara in span)
        {
            if (tabPending) { WriteIndent(builder, indent, level); tabPending = false; }
            if (!determinator.Transition(chara))
            {
                switch (determinator.Category)
                {
                    case NewLineCategory.None: break;
                    case NewLineCategory.CR: this.FlushIndent(); break;
                    default: tabPending = true; break;
                }
                determinator.Reset();
            }
            builder.Append(chara);
        }
        this.tabPending = tabPending;
        this.determinator = determinator;
    }

    public IndentedWriter Line()
    {
        this.builder.Append(this.CoreNewLine);
        this.tabPending = true;
        return this;
    }
    public void End()
    {
    }

    public IndentedWriter Indent(int delta)
    {
        this.IndentLevel += delta;
        return this;
    }
    void FlushIndent()
    {
        WriteIndent(this.builder, this.IndentString, this.IndentLevel);
        this.determinator.Reset();
        this.tabPending = false;
    }

    public override string ToString() => this.builder.ToString();
}
