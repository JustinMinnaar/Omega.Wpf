using IronOcr;

using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

namespace Jem.IronOcrLibrary
{
    public class JemIronExtractor // : CDisposable
    {
        public const string Engine = "Iron 2022.8.7804";

        public IronTesseract Ocr { get; private set; }

        // public List<Exception> Exceptions { get; set; } = new();

        static JemIronExtractor()
        {
            // https://ironpdf.com/licensing/
            var folder = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
            var path = System.IO.Path.Combine(folder, "IronOcrLicenseKey.txt");
            var licenceKey = File.ReadAllText(path);
            IronOcr.Installation.LicenseKey = licenceKey;
        }

        public JemIronExtractor()
        {
            Ocr = new IronTesseract();

            //Ocr.Configuration.TesseractVariables["L_SEVERITY_ERROR"] = true;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    //if (Input != null) { Input.Dispose(); Input = null; }
        //}

        public async Task<OcrDocument?> TryExtractOcrDocumentAsync(string sourcePath, string? password)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException(sourcePath);

            var bytes = await File.ReadAllBytesAsync(sourcePath);
            using var input = OcrInput.FromPdf(bytes, password, DPI: 300);
            //Ocr.Configuration.EngineMode = TesseractEngineMode.LstmOnly;
            // Ocr.Configuration.TrySaveAllTesseractVariablesToFile("D:\\test.txt");
            //Ocr.Configuration.RenderSearchablePdfsAndHocr = true;
            var result = await Ocr.ReadAsync(input);

            return ExtractOcrDocument(result, sourcePath);
        }

        private OcrDocument ExtractOcrDocument(OcrResult docinfo, string sourcePath)
        {
            var oDoc = new OcrDocument
            {
                Path = sourcePath,
                Text = docinfo.Text,
                Confidence = (float)docinfo.Confidence,
                Engine = Engine,
            };
            foreach (var pageinfo in docinfo.Pages)
            {
                var oPage = oDoc.AddPage(ExtractOcrPage(pageinfo));

                if (oDoc.Rect.IsEmpty)
                {
                    var oPageWidth = oPage.Size.Width;
                    var oPageHeight = oPage.Size.Height;
                    if (oPageWidth > 0 && oPageHeight > 0)
                        oDoc.Rect = new(0, 0, oPageWidth, oPageHeight);
                }
            }
            return oDoc;
        }

        private OcrPage ExtractOcrPage(OcrResult.Page pageinfo)
        {
            var oPage = new OcrPage
            {
                Text = pageinfo.Text,
                Size = new(pageinfo.Width, pageinfo.Height),
                TextDirection = (OcrTextDirection)pageinfo.TextDirection,
            };

            foreach (var blockinfo in pageinfo.Blocks)
            {
                var oBlock = ExtractOcrBlock(blockinfo, oPage);
                oPage.AddBlock(oBlock);
            }
            return oPage;
        }

        private OcrBlock ExtractOcrBlock(OcrResult.Block blockinfo, OcrPage? oPage)
        {
            var oBlock = new OcrBlock
            {
                Text = blockinfo.Text,
                Rect = new(0, 0, blockinfo.Width, blockinfo.Height),
                TextDirection = (OcrTextDirection)blockinfo.TextDirection,
            };
            foreach (var lineinfo in blockinfo.Lines)
            {
                var oLine = ExtractOcrLine(lineinfo, oPage);
                oBlock.AddLine(oLine);
            }
            return oBlock;
        }

        private OcrLine ExtractOcrLine(OcrResult.Line lineinfo, OcrPage? oPage)
        {
            var oLine = new OcrLine()
            {
                Text = lineinfo.Text,
                Rect = new CRect(lineinfo.X, lineinfo.Y, lineinfo.Width, lineinfo.Height),
                TextDirection = (OcrTextDirection)lineinfo.TextDirection,
            };
            foreach (var wordinfo in lineinfo.Words)
            {
                var oWord = ExtractOcrWord(wordinfo, oPage);
                oLine.AddWord(oWord);
            }
            return oLine;
        }

        private OcrWord ExtractOcrWord(OcrResult.Word wordinfo, OcrPage? oPage)
        {
            var oWord = new OcrWord
            {
                Text = wordinfo.Text,
                Size = new(wordinfo.Width, wordinfo.Height),
                TextDirection = (OcrTextDirection)wordinfo.TextDirection,
            };
            OcrSymbol? lastSymbol = null;
            foreach (var charinfo in wordinfo.Characters)
            {
                var oSymbol = ExtractOcrSymbol(charinfo, oPage);

                if (lastSymbol != null)
                {
                    //var distanceV = Math.Abs( oSymbol.Rect.Top - lastSymbol.Rect.Top);
                    //var distanceH = oSymbol.Rect.Left - lastSymbol.Rect.Left;
                    //if (distanceV == 0 && distanceH == 0)
                    //{
                    //    oWord.RemoveSymbol(lastSymbol);
                    //}
                }
                oWord.AddSymbol(oSymbol);
                lastSymbol = oSymbol;
            }
            return oWord;
        }

        private OcrSymbol ExtractOcrSymbol(OcrResult.Character charinfo, OcrPage? page)
        {
            var font = charinfo.Font;
            var fontIndex = page?.GetFontIndex(font?.FontName) ?? -1;
            var fontSize = (float)(font?.FontSize ?? 0f);
           
            var oSymbol = new OcrSymbol
            {
                Text = charinfo.Text,
                Confidence = (float)charinfo.Confidence,
                Rect = new CRect(charinfo.X, charinfo.Y, charinfo.Width, charinfo.Height),
                TextDirection = (OcrTextDirection)charinfo.TextDirection,
                FontIndex = fontIndex,
                FontSize = fontSize, 
            };
            return oSymbol;
        }
    }
}