using Jem.IronOcrLibrary;
using Jem.OcrLibrary22;

var folder = @"D:\BDO\Templates\Peter\";
var filePath = folder + @"9aba3e94-5d1c-4b9b-a496-70eec3f65c81_BKS_0005f2a2-96e9-43af-974c-d-7054_AUDIT 7.pdf";
Console.WriteLine(filePath);

var iron = new JemIronExtractor();

var oDoc = await iron.TryExtractOcrDocumentAsync(filePath, null);
oDoc!.Clean();

var ocrPath = folder + @"9aba3e94-5d1c-4b9b-a496-70eec3f65c81_BKS_0005f2a2-96e9-43af-974c-d-7054_AUDIT 7.bocr";
var textBefore = oDoc.Text;
await oDoc.SaveToBinaryFileAsync(ocrPath);

oDoc = await OcrDocument.LoadFromBinaryFileAsync(ocrPath);
var textAfter = oDoc.Text;

if (textAfter != textBefore)
    Console.WriteLine("Botes!");
else
    Console.WriteLine(oDoc.Text);