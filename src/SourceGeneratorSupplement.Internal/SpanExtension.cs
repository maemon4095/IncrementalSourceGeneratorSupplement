using System;

namespace SourceGeneratorSupplement.Internal;

public static class SpanExtension
{
    public static ReadOnlySpanLines EnumerateLines(this string text)
    {
        return new ReadOnlySpanLines(text.AsSpan());
    }

    public static ReadOnlySpanLines EnumerateLines(in this ReadOnlySpan<char> text)
    {
        return new ReadOnlySpanLines(in text);
    }
}