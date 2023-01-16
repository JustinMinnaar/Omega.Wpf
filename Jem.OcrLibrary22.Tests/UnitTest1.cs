using Jem.IronOcrLibrary;

namespace Jem.OcrLibrary22.Tests
{
    [TestClass]
    public class OcrUnitTest1
    {

        [TestMethod]
        public async Task CanPerformOcrFromImageFile()
        {
            var samples = new BdoSamples();
            var iron = new JemIronExtractor();

            var oDoc = await iron.TryExtractOcrDocumentAsync(samples.PathThandoCertificate1Pdf, null);
            if (oDoc == null) return;

            var textBefore = oDoc.Text;
            var ocrPath = samples.Path_ThandoCertificate1Ocr;
            if (!File.Exists(ocrPath))
                await oDoc.SaveToBinaryFileAsync(ocrPath);

            oDoc.Clean();
            //oDoc.SavePageBitmaps(true);

            oDoc = await OcrDocument.LoadFromBinaryFileAsync(ocrPath);
            var textAfter = oDoc.Text;

            Assert.AreEqual(textBefore, textAfter);
        }

        [TestMethod]
        public async Task CanPerformOcrFromPdfFilesAndLoadAndSaveOcrSymbols()
        {
            var samples = new BdoSamples();
            var iron = new JemIronExtractor();

            var oDoc = await iron.TryExtractOcrDocumentAsync(samples.PathPdf_Shadrack_Consent, null);
            var textBefore = oDoc!.Text;

            var ocrPath = samples.PathOcr_Shadrack_Consent;
            if (!File.Exists(ocrPath))
                await oDoc.SaveToBinaryFileAsync(ocrPath);

            oDoc.Clean();
            // oDoc.SavePageBitmaps(true);

            oDoc = await OcrDocument.LoadFromBinaryFileAsync(ocrPath);
            var textAfter = oDoc.Text;

            Assert.AreEqual(textBefore, textAfter);
        }
    }
}