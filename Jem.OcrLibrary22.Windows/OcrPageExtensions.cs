using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

using System.Diagnostics;
using System.Drawing;

namespace Jem.OcrLibrary22.Windows;

public static class OcrExtensions
{
    public static void SavePageBitmaps(this OcrDocument oDocument, bool overwrite = false, bool startPaintDotNet = false)
    {
        foreach (var page in oDocument.Pages)
        {
            page.SaveBitmap(oDocument, overwrite, startPaintDotNet);
        }
    }

    public static void SaveBitmap(this OcrPage oPage, OcrDocument oDocument, bool overwrite = false, bool startPaintDotNet = false)
    {
        var oldPngPath = oPage.OldDebugPath(oDocument).Replace(".bocr", ".pdf");
        var pngPath = oPage.DebugPath(oDocument).Replace(".bocr", ".pdf");
        if (File.Exists(oldPngPath))
        {
            if (File.Exists(pngPath))
                File.Delete(oldPngPath);
            else
                File.Move(oldPngPath, pngPath);
        }

        if (overwrite || !File.Exists(pngPath))
        {
            var date = new FileInfo(pngPath).LastWriteTime;
            if (date < new DateTime(2022, 12, 12, 09, 59, 00))
            {
                using var bmp = oPage.ToBitmap();
                bmp.SavePng(pngPath);

                if (startPaintDotNet)
                {
                    var paintdotnet = @"C:\Program Files\paint.net\paintdotnet.exe";
                    Process.Start(paintdotnet, pngPath.QuotedDouble()!);
                    Thread.Sleep(1000);
                }
            }
        }
    }

    public static Bitmap ToBitmap(this OcrPage oPage, 
        bool hideSymbols = false, bool hideBorders = false, bool drawImages = false, 
        bool inverse = false, bool regularFont = false)
    {
        var (pageWidth, pageHeight) = oPage.Size.Ceiling();

        //CRect bound = new();
        //foreach (var symbol in oPage.Symbols)
        //{
        //    if (bound.IsEmpty) bound = symbol.Rect;
        //    else bound = symbol.Rect.Union(bound);
        //}

        //// we can generate an empty page if there are no images or symbols
        //// if (bound.IsEmpty) throw new OcrPageException($"No ocr data found for page {oPage}!");
        //if (pageWidth < bound.Width) pageWidth = (int)Math.Ceiling(bound.Width);
        //// throw new OcrPageException($"Symbols outside bounds of width {pageWidth} for {oPage}!");
        //if (pageHeight < bound.Height) pageHeight = (int)Math.Ceiling(bound.Height);
        ////throw new OcrPageException($"Symbols outside bounds of height {pageHeight} for {oPage}!");

        ////var adjustX = Math.Min(0, bound.Left);
        ////var adjustY = Math.Min(0, bound.Top);
        ///

        var bmp = new Bitmap(pageWidth, pageHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        using var g = Graphics.FromImage(bmp);
        if (inverse)
            g.Clear(Color.Black);
        else
            g.Clear(Color.White);

        if (drawImages)
            DrawImages(oPage, pageWidth, pageHeight, inverse);

        if (!hideBorders)
            DrawBorders(oPage, g, inverse);

        if (!hideSymbols)
            DrawSymbols(oPage, g, inverse, regularFont);

        return bmp;
    }

    private static void DrawImages(OcrPage oPage, int pageWidth, int pageHeight, bool inverse)
    {
        foreach (var image in oPage.Images)
        {
            var rect = image.Rect.ToRectangle();
            if (rect.Right < 0) continue;
            if (rect.Bottom < 0) continue;
            if (rect.Left > pageWidth) continue;
            if (rect.Top > pageHeight) continue;
            //using var imageBmp = new Bitmap(image.Path);
            // todo: g.DrawImage(imageBmp, rect);
        }
    }

    private static void DrawSymbols(OcrPage oPage, Graphics g, bool inverse, bool regularFont)
    {
        //double averageFontSize = 0f;
        //int averageFontCount = 0;
        //foreach (var block in oPage.Blocks)
        //    foreach (var line in block.Lines)
        //        foreach (var word in line.Words)
        //            foreach (var symbol in word.Symbols)
        //            {
        //                averageFontSize = averageFontSize + symbol.FontSize;
        //                averageFontCount++;
        //            }
        //if (averageFontCount > 0)
        //    averageFontSize /= averageFontCount;

        //using var largeFont = new Font("Courier New", 8);
        using var mediumFont = new Font("Calibri", 18, regularFont ? FontStyle.Regular : FontStyle.Bold);
        //using var tinyFont = new Font("Courier New", 6);

        foreach (var block in oPage.Blocks)
            foreach (var line in block.Lines)
                foreach (var word in line.Words)
                    foreach (var symbol in word.Symbols)
                    {
                        if (char.IsWhiteSpace(symbol.Text[0]))
                            continue;

                        var color = Color.FromArgb(255, symbol.Color.R, symbol.Color.G, symbol.Color.B);

                        if (color.R > 50 && color.G > 50 && color.R > 50)
                        {
                            var fillColor = Color.FromArgb(255, Color.LightGray);
                            var brush = new SolidBrush(fillColor);
                            g.FillRectangle(brush, symbol.Rect.Inflated(-2, -2).ToRectangle());
                        }

                        if (inverse)
                            color = Color.FromArgb(255, 255 - color.R, 255 - color.G, 255 - color.B);

                        //var bFont = (color.R > 0 || color.G > 0 || color.B > 0) ? largeFont : smallFont;
                        // var fontSize = symbol.FontSize > 8 ? symbol.FontSize > 16 ? 11f : 9f : 7f;

                        {
                            //var bFont = tinyFont;
                            //if (symbol.Rect.Height > 8) bFont = smallFont;
                            //if (symbol.FontSize > 16) bFont = largeFont;
                            var bFont = mediumFont;

                            var brush = new SolidBrush(color);
                            var drawSize = g.MeasureString(symbol.Text, bFont);
                            var p = new PointF
                            {
                                X = (float)(symbol.Rect.Left + (symbol.Rect.Width / 2 - drawSize.Width / 2)),
                                Y = (float)(line.Rect.Bottom - drawSize.Height + 2)
                                //Y = (float) word.Rect.Top
                            };
                            g.DrawString(symbol.Text, bFont, brush, p);
                        }
                    }
    }

    private static void DrawBorders(OcrPage oPage, Graphics g, bool inverse)
    {
        var alpha = 155;

        var blockPen = new Pen(Color.FromArgb(alpha, 20, 20, 20), 0.25f);
        var linePen = new Pen(Color.FromArgb(alpha, 30, 30, 30), 0.25f);
        var wordPen = new Pen(Color.FromArgb(alpha, 40, 40, 40), 0.25f);
        var symbolPen = new Pen(Color.FromArgb(alpha, 50, 50, 50), 0.15f);
        if (!inverse)
        {
            blockPen = new Pen(Color.FromArgb(alpha, 235, 235, 235), 0.25f);
            linePen = new Pen(Color.FromArgb(alpha, 225, 225, 225), 0.25f);
            wordPen = new Pen(Color.FromArgb(alpha, 215, 215, 215), 0.25f);
            symbolPen = new Pen(Color.FromArgb(alpha, 205, 205, 205), 0.15f);
        }
        foreach (var block in oPage.Blocks)
        {
            g.DrawRectangle(blockPen, block.Rect.ToRectangle()); // Inflated(4, 4).
            foreach (var line in block.Lines)
            {
                g.DrawRectangle(linePen, line.Rect.ToRectangle()); // Inflated(2, 2).
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