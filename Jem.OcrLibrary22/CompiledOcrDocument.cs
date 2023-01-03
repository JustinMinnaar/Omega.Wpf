namespace Jem.OcrLibrary22;

public sealed class CompiledOcrDocument
{
    public CompiledOcrDocument()
    {
    }

    public CompiledOcrDocument(OcrDocument oDocument)
    {
        Name = oDocument.Name;
        Path = oDocument.Path;
        
        foreach (var oPage in oDocument.Pages)
        {
            var cPage = new CompiledOcrPage(oPage);
            Pages.Add(cPage);
        }
    }

    public string? Name { get; set; }
    public string? Path { get; set; }
    public List<CompiledOcrPage> Pages { get; } = new();

    public static async Task<CompiledOcrDocument> LoadFromBinaryFileAsync(string binOcrFilePath)
    {
        if (!File.Exists(binOcrFilePath))
            throw new FileNotFoundException(binOcrFilePath);

        var cDocument = await TryLoadFromBinaryFileAsync(binOcrFilePath);
        if (cDocument == null)
            throw new UnknownOcrFormat(binOcrFilePath);

        return cDocument;
    }

    public static async Task<CompiledOcrDocument?> TryLoadFromBinaryFileAsync(string binOcrFilePath)
    {
        try
        {
            if (!File.Exists(binOcrFilePath)) return null;

            var bytes = await File.ReadAllBytesAsync(binOcrFilePath);
            using var stream = new MemoryStream(bytes);
            using var br = new BinaryReader(stream);

            var cDocument = new CompiledOcrDocument();
            cDocument.ReadBinary(br);

            return cDocument;
        }
        catch
        {
            return null;
        }
    }


    public async Task SaveToBinaryFileAsync(string ocrFilePath)
    {
        var stream = new MemoryStream();
        var bs = new BinaryWriter(stream);
        WriteBinary(bs);
        await File.WriteAllBytesAsync(ocrFilePath, stream.ToArray());
    }

    public void ReadBinary(BinaryReader br)
    {
        Name = br.ReadString();
        var countPages = br.ReadInt32();
        for (int i = 0; i < countPages; i++)
        {
            var page = new CompiledOcrPage(br);
            Pages.Add(page);
        }
    }

    public void WriteBinary(BinaryWriter bw)
    {
        bw.Write("" + Name);
        bw.Write(Pages.Count);
        foreach (var page in Pages)
        {
            page.WriteBinary(bw);
        }
    }

}
