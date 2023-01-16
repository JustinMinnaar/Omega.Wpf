namespace Jem.PdfProcessingLibrary;

using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class JemTextLocationStrategy : LocationTextExtractionStrategy
{
    public static Dictionary<string, string> pathsByImageChecksum = new();
    public static int countDavidSpearmanLettingLogos;
    public static int countStandardBankLogos;
    public static int countAbsaLogos;

    public OcrDocument? CurrentDocument { get; set; }

    public OcrPage? CurrentPage { get; set; }

    public override void EventOccurred(IEventData data, EventType type)
    {
        if (type == EventType.RENDER_IMAGE)
        {
            EventOccurredForRenderImage((ImageRenderInfo)data);
            return;
        }

        else if (type.Equals(EventType.RENDER_TEXT))
        {
            EventOccurredForRenderText((TextRenderInfo)data);
            return;
        }

        else if (type.Equals(EventType.RENDER_PATH))
        {
            EventOccurredForRenderPath((PathRenderInfo)data);
        }

        else
        {

        }

    }

    private void EventOccurredForRenderPath(PathRenderInfo data)
    {
        var path = data.GetPath();
        var subpaths = path.GetSubpaths();
    }

    private OcrBlock? oBlock;
    // todo: private double positionY;
    private CSize blockBorder = new CSize(30, 30);

    private void EventOccurredForRenderText(TextRenderInfo textRenderInfo)
    {
        if (CurrentPage == null) return;

        double pageHeight = CurrentPage.Size.Height;

        string lineText = textRenderInfo.GetText();
        Vector lineStart = textRenderInfo.GetBaseline().GetStartPoint();
        Vector lineEnd = textRenderInfo.GetAscentLine().GetEndPoint();
        double lineLeft = lineStart.Get(0);
        double lineTop = lineStart.Get(1);
        double lineRight = lineEnd.Get(0);
        double lineBottom = lineEnd.Get(1);
        double lineWidth = lineRight - lineLeft;
        double lineHeight = lineBottom - lineTop;

        // switch from bottom up to top-down co-ordinates 
        lineTop = pageHeight - lineBottom;
        lineBottom = lineTop + lineHeight;

        // Does this line touch the existing block?
        var lineRect = new CRect(lineLeft, lineTop, lineWidth, lineHeight);

        // start a new block only when there is a gap from the last block (height of a line x 1.5)
        blockBorder = new(lineRect.Height * 3, lineRect.Height * 3);
        if (this.oBlock == null || !this.oBlock.Bounds.Inflated(blockBorder).IntersectsWith(lineRect))
            oBlock = CurrentPage.AddBlock(new OcrBlock());

        OcrLine? oLine = null;
        foreach (var oldLine in oBlock.Lines)
        {
            if (oldLine.Rect.Bottom < lineTop) continue;
            if (oldLine.Rect.Top > lineBottom) continue;
            oLine = oldLine; break;
        }

        // create a line to hold the text we are
        if (oLine == null) oLine = oBlock.AddLine(new OcrLine(lineText, lineRect, lineRect.Size));

        oBlock.Rect = oBlock.Rect.Union(oLine.Rect);

        // the line is a collection of words
        OcrWord? oWord = oLine.Words.LastOrDefault();

        var font = textRenderInfo.GetFont();
        var program = font.GetFontProgram();
        var fontName = program.ToName();
        var curFontSize = textRenderInfo.GetFontSize();

        var fc = textRenderInfo.GetFillColor();
        var sc = textRenderInfo.GetStrokeColor();


        OcrColor fillColor = GetColorFromPdfColor(fc);
        if (fillColor.R != 0 || fillColor.G != 0 || fillColor.B != 0)
        {

        }
        OcrColor strokeColor = GetColorFromPdfColor(sc);
        if (strokeColor.R != 0 || strokeColor.G != 0 || strokeColor.B != 0)
        {

        }

        IList<TextRenderInfo> renderInfos = textRenderInfo.GetCharacterRenderInfos();
        foreach (TextRenderInfo renderInfo in renderInfos)
        {
            string text = renderInfo.GetText();

            if (text == null || text.Length == 0) continue;
            if (text == " ") { oWord = null; continue; }

            bool reversed = (renderInfo.IsReversedChars());

            Vector textStart = renderInfo.GetBaseline().GetStartPoint();
            Vector textEnd = renderInfo.GetAscentLine().GetEndPoint();
            double textLeft = textStart.Get(0);
            double textTop = textStart.Get(1);
            double textRight = textEnd.Get(0);
            double textBottom = textEnd.Get(1);
            double textWidth = textRight - textLeft;
            double textHeight = textBottom - textTop;

            var startPoint = renderInfo.GetBaseline().GetStartPoint();
            var start = new CPoint(startPoint.Get(0), startPoint.Get(1));

            var endPoint = renderInfo.GetBaseline().GetEndPoint();
            var end = new CPoint(endPoint.Get(0), endPoint.Get(1));

            var diff = endPoint.Subtract(startPoint);
            var diffX = diff.Get(0);
            var diffY = diff.Get(1);

            if (end.Y != start.Y)
            {

            }


            //var length = diff.Length();

            //foreach(var tag in renderInfo.GetCanvasTagHierarchy())
            //{               

            //}

            // switch from bottom up to top-down co-ordinates 
            textTop = pageHeight - textBottom;
            textBottom = textTop + textHeight;

            var fontIndex = CurrentPage.GetFontIndex(fontName);
            var rect = new CRect(textLeft, textTop, textRight - textLeft, textHeight);

            if (oWord != null)
                if (oWord.Bounds.Right + oWord.SingleSpaceWidth / 2 < rect.Left)
                    oWord = null;

            if (oWord == null)
            {
                oWord = new OcrWord();
                oWord.Text = text;
                oWord.Rect = rect;
                oWord.SingleSpaceWidth = renderInfo.GetSingleSpaceWidth();
                oLine.AddWord(oWord);
            }
            else
            {
                oWord.Text += text;
                oWord.Rect = oWord.Rect.Union(rect);
            }


            //OcrColor fillColor = GetColorFromPdfColor(renderInfo.GetFillColor());
            //if (fillColor.R != 0 || fillColor.G != 0 || fillColor.B != 0)
            //{

            //}
            //OcrColor strokeColor = GetColorFromPdfColor(renderInfo.GetStrokeColor());
            //if (strokeColor.R != 0 || strokeColor.G != 0 || strokeColor.B != 0)
            //{

            //}
            var leading = renderInfo.GetLeading();
            var rise = renderInfo.GetRise();
            var wordSpacing = renderInfo.GetWordSpacing();
            var horizontalScaling = renderInfo.GetHorizontalScaling();

            var oSymbol = new OcrSymbol
            {
                Text = text,
                Rect = rect,
                BaseLineBegin = new CPoint(startPoint.Get(0), startPoint.Get(1)),
                BaseLineEnd = new CPoint(endPoint.Get(0), endPoint.Get(1)),
                FontIndex = fontIndex,
                FontSize = curFontSize,
                SpaceWidth = renderInfo.GetSingleSpaceWidth() / 2f,
                TextDirection = reversed ? OcrTextDirection.RightToLeft : OcrTextDirection.LeftToRight,

                Color = fillColor,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                Leading = leading,
                Rise = rise,
                WordSpacing = wordSpacing,
                HorizontalScaling = horizontalScaling,

            };
            oWord.AddSymbol(oSymbol);
        }

        if (oWord != null && oWord.Symbols.Count == 0) { oLine.RemoveWord(oWord); }

        if (oLine != null && oLine.Words.Count == 0) { oBlock.RemoveLine(oLine); }

        if (oBlock != null && oBlock.Lines.Count == 0) { CurrentPage.RemoveBlock(oBlock); }
    }

    private OcrColor GetColorFromPdfColor(Color color)
    {
        var values = color.GetColorValue();

        var numberOfComponents = color.GetNumberOfComponents();
        if (numberOfComponents == 1)
        {
            var g = (byte)(255f * values[0]);
            return new(g, g, g);
        }

        if (numberOfComponents == 3)
        {
            return new((byte)(255f * values[0]), (byte)(255f * values[1]), (byte)(255f * values[2]));
        }

        if (numberOfComponents == 4)
        {
            // 3 is alpha
            return new((byte)(255f * values[0]), (byte)(255f * values[1]), (byte)(255f * values[2]));
        }

        return new();
    }

    private void EventOccurredForRenderImage(ImageRenderInfo data)
    {
        if (CurrentPage == null) return;

        var imageName = IdentifyImage(CurrentDocument!, CurrentPage!, data);
        if (imageName == ImageClassification.Ocr)
        {
            CurrentPage.CountImagesClassifiedAsOcr++;
        }
    }

    private enum ImageClassification
    {
        None, Ocr,
        AbsaLogo,
        StandardBankLogo,
        DavidSpearmanLettingLogo,
    }

    private ImageClassification IdentifyImage(OcrDocument ocrDocument, OcrPage oPage, ImageRenderInfo renderInfo)
    {
        var image = renderInfo.GetImage();
        if (image == null) return ImageClassification.None;

        var imageResourceName = renderInfo.GetImageResourceName();
        if (imageResourceName == null) return ImageClassification.None;

        var t = renderInfo.GetGraphicsState();
        if (t == null) return ImageClassification.None;

        var imageNumber = image.GetPdfObject().GetIndirectReference().GetObjNumber();
        var extension = image.IdentifyImageFileExtension();
        var imagePath = $"{ocrDocument.Path}-{oPage.Number:000}-{imageNumber}.{extension}";
        //var imageOcrPath = $"{ocrDocument.Path}-{oPage.Number:000}-{imageNumber}.{extension}.bocr";

        // We only save ocr images for further processing
        if (File.Exists(imagePath))
            File.Delete(imagePath);
        //if (File.Exists(imageOcrPath))
        //    File.Delete(imageOcrPath);

        //if (File.Exists(imageOcrPath))
        //    return ImageClassification.Ocr;

        // You can access various value from dictionary here:
        PdfString decodeParamsPdfStr = image.GetPdfObject().GetAsString(PdfName.DecodeParms);
        string? decodeParams = decodeParamsPdfStr != null ? decodeParamsPdfStr.ToUnicodeString() : null;

        var left = renderInfo.GetStartPoint().Get(0);
        var top = renderInfo.GetStartPoint().Get(1);
        var width = image.GetWidth();
        var height = image.GetHeight();
        var size = width * height;
        if (size <= 22 * 22) return ImageClassification.None;
        if (width <= 1600) return ImageClassification.None;
        if (height <= 1600) return ImageClassification.None;

        byte[] imageBytes = image.GetImageBytes(true);
        using var s = new MemoryStream(imageBytes);

        // no need to write the image if we don't want to analyse it
        var checksum = s.GetSha512Buffered();
        switch (checksum)
        {
            case "937181FAB00E6CF6F8B92C6AB864971331BFFD962B8B9ADF14AA0DACA31B45F6":
                countAbsaLogos++;
                return ImageClassification.AbsaLogo;
            case "D33BE7E26F0FE9C24B893804B67B7F97F2283EE61AA61BBE434ADC109554948B":
                countStandardBankLogos++;
                return ImageClassification.StandardBankLogo;
            case "83839662343CD127C61B257AC23BE45416390115FABF97FF355326F841F42A8B":
                countDavidSpearmanLettingLogos++;
                return ImageClassification.DavidSpearmanLettingLogo;
            case "978F6BED1D808982BAADCA6B2E53BA60EEB6DA8BB09E215E688EE03955738F6E": // border
            case "82DD35D62018D2387BB559CA4D19FAB3F4B60B33E27D526720B3B0E215C5DEE6": // border
            case "C44AF8F5EA37C9FC3243ACCE63DFFBADF33EC21A36ED8A31E2317AA8907EFEC4": // totoya financial services
                return ImageClassification.None;
        }

        return ImageClassification.None;

        //// no need to write the image if it already exists
        //lock (pathsByImageChecksum)
        //{
        //    if (pathsByImageChecksum.TryGetValue(checksum, out var path))
        //    {
        //        imagePath = path;
        //    }
        //    else
        //    {
        //        pathsByImageChecksum.Add(checksum, imagePath);
        //        if (!File.Exists(imagePath))
        //            File.WriteAllBytes(imagePath, imageBytes);
        //    }
        //}

        //var oImage = new OcrImage
        //{
        //    Path = imagePath,
        //    Rect = new(left, top, width, height),
        //    Checksum = checksum,
        //    OcrDataFound = false,
        //};
        //oPage.AddImage(oImage);

        ////var iron = new QiiaqaIronExtractor();
        ////var ocrinfo = await iron.ExtractAsync(imagePath);
        ////if (ocrinfo != null)
        ////{
        //// oPage.SaveToBinaryFile(imageOcrPath);
        //// todo: iron.UpdatePageOcrWithImageOcrResults(oPage, ocrinfo);
        ////}

        //return ImageClassification.Ocr;
    }
}
