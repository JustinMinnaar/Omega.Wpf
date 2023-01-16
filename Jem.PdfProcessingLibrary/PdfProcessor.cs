namespace Jem.PdfProcessingLibrary;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Licensing.Base;

using Jem.IronOcrLibrary;
using Jem.OcrLibrary22;

using System.Runtime.CompilerServices;
using System.Text;

public class PdfProcessor
{
    static PdfProcessor()
    {
        // https://itextpdf.com/how-buy/agpl-license
        // https://itextpdf.com/get-started
        // Contact them at marketing@itextpdf.com in case of license issues.
        LicenseKey.LoadLicenseFile(new FileInfo("iTextLicense.json"));
    }

    public PdfProcessor()
    {
        this.OcrVersion = 1.3f;
    }

    public float OcrVersion { get; set; }
    public bool ExtractFromImagePages { get; set; }

    public bool AlwaysResizeTo2100 = false;
    public bool AlwaysOverwrite = false;

    public const string? Engine = "iText 7.2.3";


    public async Task<PdfResult> ProcessPdfAsync(string pdfFilePath, string? password)
    {
        if (!pdfFilePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidFilenameException("Expected filename to end with .pdf!");

        var result = new PdfResult { Path = pdfFilePath, Outcome = PdfOutcome.Ok };
        try
        {
            result.Document = await LoadOrPerformOcrAsync(result, pdfFilePath, password);
            return result;
        }
        catch (Exception ex)
        {
            result.Exception = ex;
            result.Outcome = PdfOutcome.Exception;
            return result;
        }
    }

    private async Task<OcrDocument?> LoadOrPerformOcrAsync(PdfResult result, string pdfFilePath, string? password)
    {
        var oDocument = await LoadOrPerformOcrAsyncInternal(result, pdfFilePath, password);

        if (oDocument != null)
        {
            if (AlwaysResizeTo2100)
                oDocument.ResizeTo2100();
            oDocument.Compile();
        }

        return oDocument;
    }

    private async Task<OcrDocument?> LoadOrPerformOcrAsyncInternal(PdfResult result, string pdfFilePath, string? password)
    {
        var folder = Path.GetDirectoryName(pdfFilePath)
            ?? throw new ArgumentNullException($"Folder not specified for path " + pdfFilePath);
        var filename = Path.GetFileNameWithoutExtension(pdfFilePath);
        var ocrFilePath = Path.Combine(folder, filename + ".bocr");

        //var ext = Path.GetExtension(pdfFilePath);

        if (!AlwaysOverwrite && File.Exists(ocrFilePath))
        {
            var oDocument = await OcrDocument.TryLoadFromBinaryFileAsync(ocrFilePath);
            if (oDocument != null)
            {
                if (oDocument.Pages.Count == 0 || oDocument.Pages[0].SymbolsCount == 0)
                    oDocument = null;
                else if (oDocument.ProcessingVersion < OcrVersion)
                    oDocument = null;
            }
            if (oDocument != null) return oDocument;
        }

        using (var pdfDoc = TryOpenPdfDocument(result, pdfFilePath, password))
        {
            if (pdfDoc == null) return null;
            if (pdfDoc.IsClosed()) return null;

            if (pdfDoc.GetPage(0).GetRotation() != 0)
            {

            }
            ExtractFromPdf(result, pdfDoc, pdfFilePath, password);
        }

        var oNewDocument = result.Document;
        if (oNewDocument == null) return null;

        if (oNewDocument.Pages.Count == 0) return null;

        var countSymbolsOnAllPages = oNewDocument.Pages.Sum(a => a.SymbolsCount);
        if (countSymbolsOnAllPages == 0)
        {
            if (ExtractFromImagePages)
            {
                var iron = new JemIronExtractor();
                var oDoc = await iron.TryExtractOcrDocumentAsync(pdfFilePath, password);
                if (oDoc == null) return null;
                oNewDocument = oDoc;

                countSymbolsOnAllPages = oNewDocument.Pages.Sum(a => a.SymbolsCount);
                if (countSymbolsOnAllPages == 0)
                {
                    result.Outcome = PdfOutcome.Empty;
                    return null;
                }
            }
            else
                return null;
        }

        oNewDocument.ProcessingVersion = OcrVersion;

        oNewDocument.SaveToBinaryFile(ocrFilePath);
        try
        {
            oNewDocument = await OcrDocument.LoadFromBinaryFileAsync(ocrFilePath);
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }

        return oNewDocument;
    }

    static PdfDocument? TryOpenPdfDocument(PdfResult result, string pdfFilePath, string? password)
    {
        try
        {
            // try first without password
            PdfReader reader = new PdfReader(pdfFilePath);
            var pdfDoc = new PdfDocument(reader);
            return pdfDoc;
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("Bad user password"))
            {
                if (password != null)
                {
                    Encoding cp437 = Encoding.GetEncoding(437);
                    List<byte[]> passwordByteList = new List<byte[]>()
                    {
                        Encoding.Default.GetBytes(password), // Default codepage
                        Encoding.UTF8.GetBytes(password),    // UTF8 encoding
                        cp437.GetBytes(password),            // Code page 437 (extended ASCII) encoding
                    };

                    foreach (byte[] passwordBytes in passwordByteList)
                    {
                        PdfReader reader = new PdfReader(pdfFilePath, new ReaderProperties().SetPassword(passwordBytes));
                        try
                        {
                            var pdfDoc = new PdfDocument(reader);
                            return pdfDoc;
                        }
                        catch (Exception ex2)
                        {
                            //Exception thrown by PDF reader. We need to try the next password.
                            if (!ex2.Message.StartsWith("Bad user password"))
                                System.Diagnostics.Debug.WriteLine(ex2.ToString());
                        }
                    }
                }

                result.Outcome = PdfOutcome.InvalidPassword;
                return null;
            }

            result.Exception = ex;
            result.Outcome = PdfOutcome.Exception;
            return null;
        }
    }

    private void ExtractFromPdf(PdfResult result, PdfDocument pdfDoc, string pdfFilePath, string? password)
    {
        result.Document = new OcrDocument { Path = pdfFilePath, Engine = Engine, ProcessingVersion = OcrVersion };

        var size = pdfDoc.GetDefaultPageSize();
        result.Document.Rect = new CRect(0, 0, size.GetWidth(), size.GetHeight());

        for (int pageNumber = 1; pageNumber <= pdfDoc.GetNumberOfPages(); pageNumber++)
        {
            var listener = new FilteredEventListener();
            var strat = listener.AttachEventListener(new JemTextLocationStrategy());
            strat.CurrentDocument = result.Document;

            var processor = new PdfCanvasProcessor(listener);
            try
            {
                var pdfPage = pdfDoc.GetPage(pageNumber);
                var pageSize = pdfPage.GetPageSize();

                var oPage = new OcrPage { Number = "" + pageNumber, Size = new CSize(pageSize.GetWidth(), pageSize.GetHeight()) };
                result.Document.AddPage(oPage);
                strat.CurrentPage = oPage;

                processor.ProcessPageContent(pdfPage);

                var rotation = pdfPage.GetRotation();
                oPage.Rotate(rotation);

                if (oPage.IsEmpty)
                    result.Outcome = PdfOutcome.Empty;
                else
                    result.Outcome = PdfOutcome.Ok;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Outcome = PdfOutcome.Exception;
            }
        }
    }

    //private void ExtractImagesFromPdfDocument(Result result, PdfDocument pdfDoc, PdfPage pdfPage, string pdfFilePath)
    //{
    //    int imageIndex = 1;
    //    var resouces = pdfPage.GetResources();
    //    var resourceNames = resouces.GetResourceNames().ToList();

    //    foreach (var resourcename in resourceNames)
    //    {
    //        try
    //        {
    //            var picImage = resouces.GetImage(resourcename);
    //            if (picImage == null) continue;

    //            var type = picImage.IdentifyImageType();
    //            var ext = picImage.IdentifyImageFileExtension();
    //            var width = picImage.GetWidth();
    //            var height = picImage.GetHeight();

    //            if (width >= 100 && height >= 20)
    //                //var bmp = new bitmap()
    //                //var picPath = $@"{imageIndex}.png";
    //                picImage.save(picPath, ImageFormat.Png);
    //            imageIndex++;
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }
    //}

}


public class PdfResult
{
    public string? Path { get; set; }
    public PdfOutcome Outcome { get; set; }
    public bool IsIdentified { get; set; }
    public int? PageCount => Document?.PagesCount ?? 0;
    public Exception? Exception { get; set; }
    public OcrDocument? Document { get; set; }
}

public enum PdfOutcome
{
    Ok,
    InvalidPassword,
    Exception,
    Empty
}