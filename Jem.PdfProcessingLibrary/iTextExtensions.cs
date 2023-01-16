namespace Jem.PdfProcessingLibrary;

using iText.IO.Font;

public static class ITextExtensions
{
    public static string? ToName(this FontProgram program)
    {
        var names = program.GetFontNames();
        string fontName = names.GetFontName();
        if (fontName == null || fontName.Length <= 0)
        {
            return null;
        }

        return fontName;
    }
}