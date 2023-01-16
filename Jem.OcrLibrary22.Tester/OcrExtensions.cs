using Jem.CommonLibrary22;

namespace Jem.OcrLibrary22.Tester;

public static class OcrExtensions
{
    public static readonly CSize TestFontSize = new(15, 25);

    public static void AddText(this OcrPage oPage, ref CPoint p, params string[] textLines)
    {
        var block = new OcrBlock();
        block.AddText(ref p, textLines);
        oPage.AddBlock(block);
    }

    public static void AddText(this OcrBlock block, ref CPoint p, params string[] textLines)
    {
        var left = p.X;
        foreach (var textLine in textLines)
        {
            var line = new OcrLine();
            line.AddText(ref p, textLine);

            p.X = left;
            p.Y += TestFontSize.Height;
        }
    }

    public static void AddText(this OcrLine oLine, ref CPoint p, string textLine)
    {
        var textWords = textLine.Split(' ');
        var first = true;
        foreach (var textWord in textWords)
        {
            if (first) first = false;
            else
            {
                // skip a space after each word
                p.X += TestFontSize.Width;
            }

            var oWord = new OcrWord();
            oWord.AddText(ref p, textWord);
        }
    }

    public static void AddText(this OcrWord oWord, ref CPoint p, string textWord)
    {
        foreach (var textCharacter in textWord)
        {
            var rect = new CRect(p.X, p.Y, TestFontSize.Width, TestFontSize.Height);
            var symbol = new OcrSymbol(textCharacter.ToString(), rect, rect.Size);
            oWord.AddSymbol(symbol);

            p.X += TestFontSize.Width;
        }
    }
}