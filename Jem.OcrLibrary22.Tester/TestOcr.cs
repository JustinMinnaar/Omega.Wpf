using Jem.CommonLibrary22;

namespace Jem.OcrLibrary22.Tester;

public static class TestOcr
{
    public const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static OcrPage NewTestPage(params string[] lines)
    {
        var oPage = new OcrPage();
        var p = new CPoint(50, 100);
        oPage.AddText(ref p, lines);
        return oPage;
    }

    public static string? ComparePages(OcrPage original, OcrPage duplicate)
    {
        var originalSymbols = original.Symbols.ToList();
        var duplicateSymbols = duplicate.Symbols.ToList();

        if (originalSymbols.Count != duplicateSymbols.Count)
            return $"Original has {originalSymbols.Count} and duplicate has {duplicateSymbols.Count} symbols!";

        for (int i = 0; i < originalSymbols.Count; i++)
        {
            var originalSymbol = originalSymbols[i];
            var duplicateSymbol = duplicateSymbols[i];
            if (originalSymbol.ToStringDebug != duplicateSymbol.ToStringDebug)
                return $"At {i}, original '{originalSymbol.ToStringDebug}' and duplicate '{duplicateSymbol.ToStringDebug}'!";
        }

        return null;
    }

    //public static void SymbolsAreAsExpected(JOcrPage page) => SymbolsAreAsExpected(page, letters);

    //public static void SymbolsAreAsExpected(JOcrPage page, string letters)
    //{
    //    double x = 0, y = 0, w = 15, h = 25;
    //    var i = 0;
    //    foreach (var symbol in page.Symbols)
    //    {
    //        if (symbol.Character != letters[i])
    //            throw new JOcrException($"Symbol '{symbol.Character}' at index {i} should be '{letters[i]}!");

    //        var rect = new CRect(x, y, w, h);
    //        if (rect.ToString() != symbol.Rect.ToString())
    //            throw new JOcrException($"Symbol '{symbol.Character}' at index {i} not at {rect}!");
    //        x += w;
    //        y += h / 10f;
    //    }
    //}

}
