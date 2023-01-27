namespace SourceGeneratorSupplement.Internal;

internal static class SpanExtension
{
    public static ReadOnlySpanLines EnumerateLines(this string text)
    {
        return new ReadOnlySpanLines(text.AsSpan());
    }

    public static ReadOnlySpanLines EnumerateLines(this ReadOnlySpan<char> text)
    {
        return new ReadOnlySpanLines(text);
    }
}