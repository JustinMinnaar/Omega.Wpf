namespace Jem.OcrLibrary22;

public static class OcrExtensions
{
    /// <summary>The bounds of the blocks rectangles.</summary>
    public static CRect Bounds(this IEnumerable<OcrBlock> blocks)
    {
        var bounds = new CRect();
        foreach (var block in blocks)
        {
            bounds = bounds.Union(block.Rect);
        }
        return bounds;
    }

    /// <summary>The bounds of the lines rectangles.</summary>
    public static CRect Bounds(this IEnumerable<OcrLine> lines)
    {
        var bounds = new CRect();
        foreach (var line in lines)
        {
            bounds = bounds.Union(line.Rect);
        }
        return bounds;
    }

    /// <summary>The bounds of the words rectangles.</summary>
    public static CRect Bounds(this IEnumerable<OcrWord> words)
    {
        var bounds = new CRect();
        foreach (var word in words)
        {
            bounds = bounds.Union(word.Rect);
        }
        return bounds;
    }

    /// <summary>The bounds of the symbols rectangles.</summary>
    public static CRect Bounds(this IEnumerable<OcrSymbol> symbols)
    {
        var bounds = new CRect();
        foreach (var symbol in symbols)
        {
            bounds = bounds.Union(symbol.Rect);
        }
        return bounds;
    }
}
