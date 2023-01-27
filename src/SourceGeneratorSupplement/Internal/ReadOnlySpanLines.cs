namespace SourceGeneratorSupplement.Internal;

internal readonly ref struct ReadOnlySpanLines
{
    public ref struct Enumerator
    {
        public Enumerator(ReadOnlySpanLines self)
        {
            this.index = -1;
            this.remain = self.text;
        }
        int index;
        ReadOnlySpan<char> remain;

        public bool MoveNext()
        {
            var remain = this.remain;
            var index = this.index;
            if (index >= remain.Length)
            {
                return false;
            }
            if (index >= 0)
            {
                //skip CRLF
                if (index < remain.Length - 1 && remain[index] == '\r' && remain[index + 1] == '\n')
                {
                    index++;
                }
                remain = remain.Slice(index + 1);
            }
            index = remain.IndexOfAny('\r', '\n');
            this.index = index < 0 ? remain.Length : index;
            this.remain = remain;
            return true;
        }

        public ReadOnlySpan<char> Current => this.remain.Slice(0, this.index);
    }

    public ReadOnlySpanLines(ReadOnlySpan<char> text)
    {
        this.text = text;
    }

    private readonly ReadOnlySpan<char> text;

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }
}
