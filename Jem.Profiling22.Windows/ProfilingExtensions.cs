using Jem.OcrLibrary22;
using Jem.OcrLibrary22.Windows;
using Jem.Profiling22.Data;

using System.Data;
using System.Diagnostics;
using System.Drawing;

namespace Jem.Profiling22.Windows;

public static class ProfilingExtensions
{
    public static Bitmap ToBitmap(OcrPage oPage, JProProfileResult pResult, bool hideSections = false, bool hideExtractors = false, bool hideFields = false)
    {
        var bmp = oPage.ToBitmap();
        using var g = Graphics.FromImage(bmp);

        var templateResults = pResult.TemplateResults;
        foreach (var templateResult in templateResults)
            DrawTemplateResult(g,  templateResult);

        return bmp;
    }

    private static void DrawTemplateResult(Graphics g, JProTemplateResult templateResult)
    {
        var theory = templateResult.Theory;

        var pen = new Pen(Color.DarkRed);
        foreach (var rResult in templateResult.RowResults)
        {
            foreach (var eResult in rResult.ExtractorResults)
            {
                g.DrawRectangle(pen, eResult.Rect.ToRectangle());
            }
        }
    }

    private static void DrawSections(OcrPage oPage, ProProfile profile)
    {
        foreach (var image in oPage.Images)
        {
            var rect = image.Rect.ToRectangle();
            if (rect.Right < 0) continue;
            if (rect.Bottom < 0) continue;
            //if (rect.Left > pageWidth) continue;
            //if (rect.Top > pageHeight) continue;
            //using var imageBmp = new Bitmap(image.Path);
            // todo: g.DrawImage(imageBmp, rect);
        }
    }

    private static void DrawSymbols(OcrPage oPage, Graphics g)
    {
        double averageFontSize = 0f;
        int averageFontCount = 0;
        foreach (var block in oPage.Blocks)
            foreach (var line in block.Lines)
                foreach (var word in line.Words)
                    foreach (var symbol in word.Symbols)
                    {
                        averageFontSize = averageFontSize + symbol.FontSize;
                        averageFontCount++;
                    }
        if (averageFontCount > 0)
            averageFontSize /= averageFontCount;

        var largeFont = new Font("Courier New", 14);
        var smallFont = new Font("Courier New", 10);
        foreach (var block in oPage.Blocks)
            foreach (var line in block.Lines)
                foreach (var word in line.Words)
                    foreach (var symbol in word.Symbols)
                    {
                        if (char.IsWhiteSpace(symbol.Text[0]))
                            continue;


                        var color = Color.FromArgb(255, symbol.Color.R, symbol.Color.G, symbol.Color.B);
                        if (color.R > 200 && color.G > 200 && color.R > 200)
                        {
                            var fillColor = Color.FromArgb(255, Color.SaddleBrown);
                            var brush = new SolidBrush(fillColor);
                            var symbolPen = new Pen(fillColor, 1);
                            g.FillRectangle(brush, symbol.Rect.ToRectangle());
                        }

                        //var bFont = (color.R > 0 || color.G > 0 || color.B > 0) ? largeFont : smallFont;
                        var fontSize = symbol.FontSize / 12.5f;
                        if (fontSize < 8) fontSize = 8f;
                        using var bFont = new Font("Courier New", fontSize);
                        {
                            var brush = new SolidBrush(color);
                            var drawSize = g.MeasureString(symbol.Text, bFont);
                            var p = new PointF
                            {
                                X = (float)(symbol.Rect.Left + (symbol.Rect.Width / 2 - drawSize.Width / 2)),
                                Y = (float)(word.Rect.Bottom - drawSize.Height)
                            };
                            g.DrawString(symbol.Text, bFont, brush, p);
                        }
                    }
    }

    private static void DrawBorders(OcrPage oPage, Graphics g)
    {
        var borderPen = new Pen(Color.AntiqueWhite, 0.25f);
        var linePen = new Pen(Color.WhiteSmoke, 0.25f);
        var wordPen = new Pen(Color.NavajoWhite, 0.25f);
        var symbolPen = new Pen(Color.GhostWhite, 0.15f);
        foreach (var block in oPage.Blocks)
        {
            g.DrawRectangle(borderPen, block.Rect.Inflated(4, 4).ToRectangle());
            foreach (var line in block.Lines)
            {
                g.DrawRectangle(linePen, line.Rect.Inflated(2, 2).ToRectangle());
                foreach (var word in line.Words)
                {
                    g.DrawRectangle(wordPen, word.Rect.ToRectangle());
                    foreach (var symbol in word.Symbols)
                    {
                        g.DrawRectangle(symbolPen, symbol.Rect.ToRectangle());
                    }
                }
            }
        }
    }

}